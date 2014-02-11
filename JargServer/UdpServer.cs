
using System;
using System.Collections.Generic;
using System.Linq;
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
        private int inTraf, outTraf;
        

        public List<OtherClient> GetClients {
            get { return connected.Values.ToList(); }
        }

        public int GetInnerTraffic {
            get { return inTraf; }
        }

        public int GetOuterTraffic
        {
            get { return outTraf; }
        }

        public double GetInnerSpeed
        {
            get { return inSpeed; }
        }

        public double GetOuterSpeed
        {
            get { return outSpeed; }
        }

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

            sendthread = new Thread(UpdateAndSendStructs);
            sendthread.Start();

            new Thread(MonitorThread).Start();

            //logicThread = new Thread(updateLogic);
            //logicThread.Start();
        }

        public void UpdateLogic()
        {
            // Nothing
        }


        int traf1, traf2;
        double inSpeed, outSpeed;

        void MonitorThread()
        {
            while (running)
            {
                traf1 = inTraf;
                traf2 = outTraf;
                Thread.Sleep(1000);
                inSpeed = inTraf - traf1;
                outSpeed = outTraf - traf2;
            }
        }

        void UpdateAndSendStructs()
        {
            #region send messages
            while (running)
            {
                Thread.Sleep(100);

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
                        inTraf += data.Length;
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
                        case "biom":
                            GiveBiom(ds.name, ds.x, ds.y);
                            break;
                        default:
                            Console.WriteLine("Unknown message (" + ds.action + ")");
                            break;
                    }
                }

            Console.Write("Listening thread stops.");
        }

        private void GiveBiom(string name, float f, float f1) {
            StringBuilder n = new StringBuilder();
            var pack = lw_.TryGetPacked(new Point((int)f, (int)f1), gl_);
            if (pack == null) { return; }
            SendStruct(new JargPack { action = "mapsector", name = "name", mapsector = pack, x = (int)f, y = (int)f1 }, name);
        }

        private void GiveSector(string name, float f, float f1) {
            StringBuilder n = new StringBuilder();
            var pack = lw_.TryGetPacked(new Point((int)f, (int)f1), gl_);
            if(pack == null){ return; }
            SendStruct(new JargPack{action = "mapsector", name = "name", mapsector = pack, x = (int)f, y = (int) f1}, name);
        }

        public void SendStruct(JargPack msg, string name)
        {
            byte[] data = MarshalHelper.SerializeMessage(msg);
            outTraf += data.Length;
            if (connected.ContainsKey(name))
            {
                if (connected[name].ipendpoint != null)
                {
                    udp.Send(data, data.Length, connected[name].ipendpoint);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(name);
                Console.ResetColor();
                Console.Write(" could not be found");
                Console.WriteLine();
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
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(name);
            Console.ResetColor();
            Console.Write(" is trying to connect");

            lock (connected)
            {
                if (!connected.ContainsKey(name))
                {
                    // Add to connections
                    connected.Add(name, new OtherClient(name, ipep, rand.Next(0, 500), rand.Next(0, 500)));
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" Accepted");
                    Console.ResetColor();
                    Console.Write(" ({0})\n", ipep.ToString());

                    // Send out authorized.msg
                    var ds = new JargPack{action = "accept", name = name};
                    SendStruct(ds, name);

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" Rejected");
                    Console.ResetColor();
                    Console.Write(" (name exists)\n");
                }
            }
            Console.WriteLine("Total connected: " + connected.Count.ToString());
        }

        public void Close() {
            running = false;
            udp.Close();
        }
    }
}
