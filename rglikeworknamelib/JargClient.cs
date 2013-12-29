using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        public Dictionary<string, OtherClient> otherclients;

        public JargClient(string n)
        {
            name = n;
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
        }

        public void Disconnect() {
            SendStruct(new JargPack {action = "disconnect", name = name});
        }

        void ListenForStructs()
        {
            while (true)
            {
                byte[] data = udpclient.Receive(ref ipendpoint);
                object d = MarshalHelper.DeserializeMsg<JargPack>(data);
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
                                otherclients[ds.name].x = (int)ds.x;
                                otherclients[ds.name].y = (int)ds.y;
                            }
                            else
                            {
                                lock (otherclients)
                                {
                                    otherclients.Add(ds.name, new OtherClient(ds.name, ds.x, ds.y));
                                }
                            }
                            break;

                        case "aim":
                            if (otherclients.ContainsKey(ds.name))
                            {
                                otherclients[ds.name].aim_x = (int)ds.x;
                                otherclients[ds.name].aim_y = (int)ds.y;
                            }
                            break;

                        case "disconnect":
                            if (otherclients.ContainsKey(ds.name)) {
                                otherclients.Remove(ds.name);
                            }
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
