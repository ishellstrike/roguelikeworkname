using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib
{
    public class JargClient
    {
        UdpClient udpclient;
        IPEndPoint ipendpoint;
        Thread listenthread;
        bool connected;
        public int x;
        public int y;
        public string name;
        public bool running;
        public Dictionary<string, OtherClient> otherclients;
        private LevelWorker lw_;
        private GameLevel gl_;

        // Send connection sequence
        public JargClient(string n, LevelWorker lw, GameLevel gl) {
            gl_ = gl;
            lw_ = lw;
            name = n;
            running = true;
            ipendpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            udpclient = new UdpClient();
            udpclient.Connect(ipendpoint);

            listenthread = new Thread(ListenForStructs);
            listenthread.Start();

            // Connect
            JargPack connectmessage = new JargPack();
            connectmessage.action = "connect";
            connectmessage.name = name;
            SendStruct(connectmessage);
            otherclients = new Dictionary<string, OtherClient>();
        }

        public void Disconnect() {
            SendStruct(new JargPack {action = "disconnect", name = name});
            running = false;
            udpclient.Close();
        }

        void ListenForStructs()
        {
            while (running) {
                byte[] data;
                try {
                    data = udpclient.Receive(ref ipendpoint);
                }
                catch (SocketException) {
                    break;
                }
                object d = MarshalHelper.DeserializeMsg(data);
                JargPack ds = (JargPack)d;

                if (ds.action == "accept" && ds.name == name)
                {
                    connected = true;
                }

                if (connected)
                {
                    switch (ds.action)
                    {
                        case "position":
                            if (otherclients.ContainsKey(ds.name))
                            {
                                OtherClient client = otherclients[ds.name];
                                client.x = (ds.x + client.x)/2;
                                client.y = (ds.y + client.y)/2;
                                client.angle = (ds.angle + client.angle) / 2;
                            }
                            else
                            {
                                lock (otherclients)
                                {
                                    otherclients.Add(ds.name, new OtherClient(ds.name, ds.x, ds.y, ds.angle));
                                }
                            }
                            break;

                        case "disconnect":
                            if (otherclients.ContainsKey(ds.name)) {
                                otherclients.Remove(ds.name);
                            }
                            break;
                        case "mapsector":
                            lw_.StoreString(new Point((int) ds.x, (int) ds.y),  ds.mapsector);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public void SendStruct(string _action, string _name, float _x, float _y)
        {
            SendStruct(new JargPack {action = _action, name = _name, x = _x, y = _y});
        }

        public void SendStruct(JargPack s)
        {
            byte[] b = MarshalHelper.SerializeMessage(s);
            udpclient.Send(b, b.Length);
        }

        public void SendMessage(string message)
        {
            message = name + " " + message;
            byte[] data = Encoding.ASCII.GetBytes(message);
            udpclient.Send(data, data.Length);
        }

        public override string ToString()
        {
            return name + " (" + x.ToString() + "," + y.ToString() + ")";
        }
    }
}
