using System.Net;

namespace rglikeworknamelib {
    public class OtherClient
    {
        public string name;
        public float x;
        public float y;
        public float angle;
        public IPEndPoint ipendpoint ;

        public OtherClient(string n, float X, float Y, float a)
        {
            name = n;
            x = X;
            y = Y;
            angle = a;
        }

        public OtherClient(string s, IPEndPoint ipep, int next, int i) {
            name = s;
            ipendpoint = ipep;
            x = next;
            y = i;
        }
    }
}