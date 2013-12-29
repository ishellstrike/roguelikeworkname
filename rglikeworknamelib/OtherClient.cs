namespace rglikeworknamelib {
    public class OtherClient
    {
        public string name;
        public float x;
        public float y;
        public float aim_x;
        public float aim_y;

        public OtherClient(string n, float X, float Y)
        {
            name = n;
            x = X;
            y = Y;
            aim_x = 0.0f;
            aim_y = 0.0f;
        }
    }
}