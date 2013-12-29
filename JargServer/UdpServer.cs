
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using rglikeworknamelib;

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
        private Dictionary<string, ConnectedClient> connected;
        Random rand;
        private Dictionary<string, Thread> actions;

        public UDPServer()
        {
            running = false;
            connected = new Dictionary<string, ConnectedClient>();
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

        public class ConnectedClient
        {
            public string name;
            public IPEndPoint ipendpoint;
            public float x;
            public float y;
            public float aim_x;
            public float aim_y;

            public ConnectedClient(string n, IPEndPoint ipep, int X, int Y)
            {
                name = n;
                ipendpoint = ipep;
                x = X;    // sets to random when created from server..
                y = Y;
            }
        }

        public void StartServer(int port)
        {
            udp = new UdpClient(port);
            running = true;

            listenthread = new Thread(ListenAndHandleMessages);
            listenthread.Start();

            //sendthread  = new Thread(updateAndSendStructs);
            //sendthread.Start();

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
            while (true)
            {
                Thread.Sleep(50);

                if (connected.Count > 0)
                {
                    lock (connected)
                    {
                        foreach (KeyValuePair<string, ConnectedClient> c in connected)
                        {
                            foreach (KeyValuePair<string, ConnectedClient> c2 in connected)
                            {
                                if (c2.Key != c.Key)
                                {
                                    // Update position.
                                    JargPack msg = new JargPack();
                                    msg.action = "position";
                                    msg.name = c2.Key;
                                    msg.x = c2.Value.x;
                                    msg.y = c2.Value.y;
                                    SendStruct(msg, c.Key);

                                    // Update aim.
                                    msg.action = "aim";
                                    msg.x = c2.Value.aim_x;
                                    msg.y = c2.Value.aim_y;
                                    SendStruct(msg, c.Key);
                                    Console.Write("s");
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
                    byte[] data = udp.Receive(ref ipendpoint); // Get some data
                    object d = MarshalHelper.DeserializeMsg<JargPack>(data);
                    var ds = (JargPack) d;

                    if (ds.action == "connect") {
                        HandleConnection(ds.name, ipendpoint);
                    }
                    if (ds.action == "disconnect")
                    {
                        EndConnection(ds.name, ipendpoint);
                    }
                    else if (actions.ContainsKey(ds.action)) {
                        Console.WriteLine(ds.action);
                        actions[ds.action].Start();
                    }
                    else {
                        Console.WriteLine("Action is not \"connect\" (" + ds.action + ")");
                    }
                }

            Console.Write("Listening thread stops.");
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
                    connected.Add(name, new ConnectedClient(name, ipep, rand.Next(0, 500), rand.Next(0, 500)));
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
    }
}
