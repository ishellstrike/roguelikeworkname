namespace rglikeworknamelib.Dungeon.Level {
    public class Schemes {
        public string[] data;
        public SchemesType type;
        public int x, y;
        public string filename;

        public void Rotate(int rr) {
            var temp = new string[x*y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[(y - j - 1)*x + i] = data[i*y + j];
                }
            }
            int t = x;
            x = y;
            y = t;
            data = temp;
        }

        public void TransHor() {
            var temp = new string[x*y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[i*y + j] = data[((x - 1) - i)*y + j];
                }
            }
            data = temp;
        }

        public void TransVer() {
            var temp = new string[x*y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[i*y + j] = data[i*y + ((y - 1) - j)];
                }
            }
            data = temp;
        }
    }
}