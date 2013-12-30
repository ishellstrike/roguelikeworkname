
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Level;

namespace JargServer
{
    public class UDPServer
    {
        private List<Thread> threads;

        private Thread listenthread;
        private Thread sendthread;
        private Thread logicThread;
        private UdpClient udp;
        private bool running;
        private Dictionary<string, OtherClient> connected;
        Random rand;
        private Dictionary<string, Thread> actions;
        private LevelWorker lw_;
        private GameLevel gl_;

        public UDPServer(LevelWorker lw, GameLevel gl) {
            lw_ = lw;
            gl_ = gl;
            running = false;
            connected = new Dictionary<string, OtherClient>();
            rand = new Random();
            threads = new List<Thread>();
            actions = new Dictionary<string, Thread>();
        }

        // Adds a function to the server, this function is specified outside of this class
        public void AddAction(string action, ThreadStart function)
        {
            Thread t = new Thread(function);
            actions.Add(action, t);
        }

        public void StartServer(int port)
        {
            udp = new UdpClient(port);
            running = true;

            listenthread = new Thread(ListenAndHandleMessages);
            listenthread.Start();

            sendthread  = new Thread(UpdateAndSendStructs);
            sendthread.Start();

            //logicThread = new Thread(updateLogic);
            //logicThread.Start();
        }

        public void UpdateLogic()
        {
            // Nothing
        }


        void UpdateAndSendStructs(Object obj)
        {
            #region send messages
            while (running)
            {
                Thread.Sleep(50);

                if (connected.Count > 0)
                {
                    lock (connected)
                    {
                        foreach (KeyValuePair<string, OtherClient> c in connected)
                        {
                            foreach (KeyValuePair<string, OtherClient> c2 in connected)
                            {
                                if (c2.Key != c.Key)
                                {
                                    // Update position.
                                    JargPack msg = new JargPack();
                                    msg.action = "position";
                                    msg.name = c2.Key;
                                    msg.x = c2.Value.x;
                                    msg.y = c2.Value.y;
                                    msg.angle = c2.Value.angle;
                                    SendStruct(msg, c.Key);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
        }

        public void ListenAndHandleMessages()
        {
                while (running) {
                    var ipendpoint = new IPEndPoint(IPAddress.Any, 80); // Listen for anywhere
                    byte[] data;
                    try {
                        data = udp.Receive(ref ipendpoint); // Get some data
                    }
                    catch (SocketException) {
                        continue;
                    }
                    object d = MarshalHelper.DeserializeMsg(data);
                    var ds = (JargPack) d;

                    switch (ds.action) {
                        case "connect":
                            HandleConnection(ds.name, ipendpoint);
                            break;
                        case "disconnect":
                            EndConnection(ds.name, ipendpoint);
                            break;
                        case "mapsector":
                            GiveSector(ds.name, ds.x, ds.y);
                            break;
                        case "position":
                            if (connected.ContainsKey(ds.name)) {
                                connected[ds.name].x = ds.x;
                                connected[ds.name].y = ds.y;
                                connected[ds.name].angle = ds.angle;
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown message (" + ds.action + ")");
                            break;
                    }
                }

            Console.Write("Listening thread stops.");
        }

        private void GiveSector(string name, float f, float f1) {
            StringBuilder n = new StringBuilder();
            var pack = new[] {lw_.TryGet(new Point((int)f, (int)f1), gl_)};
            if(pack[0] == null){ return; }
            lw_.SectorSaver(pack, n);
            SendStruct(new JargPack{action = "mapsector", name = "name", mapsector = n.ToString()}, name);
        }

        public void SendStruct(JargPack msg, string name)
        {
            byte[] data = MarshalHelper.SerializeMessage(msg);
            if (connected.ContainsKey(name))
            {
                if (connected[name].ipendpoint != null)
                {
                    udp.Send(data, data.Length, connected[name].ipendpoint);
                }
            }
            else
            {
                #region console it
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(name);
                Console.ResetColor();
                Console.Write(" could not be found");
                Console.WriteLine();
                #endregion
            }
        }

        public void EndConnection(string name, IPEndPoint ipep)
        {
            lock (connected) {
                if (connected.ContainsKey(name)) {
                    connected.Remove(name);
                    Console.WriteLine("{0} disconnected ({1})", name, ipep);
                }
            }
        }

        public void HandleConnection(string name, IPEndPoint ipep)
        {
            #region Accept/Reject
            #region console it
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(name);
            Console.ResetColor();
            Console.Write(" is trying to connect");
            #endregion

            lock (connected)
            {
                if (!connected.ContainsKey(name))
                {
                    // Add to connections
                    connected.Add(name, new OtherClient(name, ipep, rand.Next(0, 500), rand.Next(0, 500)));
                    #region console it
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" Accepted");
                    Console.ResetColor();
                    Console.Write(" ({0})\n", ipep.ToString());
                    #endregion

                    // Send out authorized.msg
                    var ds = new JargPack{action = "accept", name = name};
                    SendStruct(ds, name);

                }
                else
                {
                    #region console it
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" Rejected");
                    Console.ResetColor();
                    Console.Write(" (name exists)\n");
                    #endregion
                }
            }
            #endregion
            Console.WriteLine("Total connected: " + connected.Count.ToString());
            //Console.Title = "Connections: " + connected.Count.ToString();
        }

        public void Close() {
            running = false;
            udp.Close();
        }
    }
}
