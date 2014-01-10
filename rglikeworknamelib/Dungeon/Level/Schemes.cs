namespace rglikeworknamelib.Dungeon.Level {
    public class Schemes {
        public string[] data;
        public string[] floor;
        public SectorBiom type;
        public int x, y;
        public string filename;

        public void Rotate(int rr) {
            var temp = new string[x * y];
            var tempf = new string[x * y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[(y - j - 1) * x + i] = data[i * y + j];
                    tempf[(y - j - 1) * x + i] = floor[i * y + j];
                }
            }
            int t = x;
            x = y;
            y = t;
            data = temp;
            floor = tempf;
        }

        public void TransHor() {
            var temp = new string[x * y];
            var tempf = new string[x * y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[i * y + j] = data[((x - 1) - i) * y + j];
                    tempf[i * y + j] = floor[((x - 1) - i) * y + j];
                }
            }
            data = temp;
            floor = tempf;
        }

        public void TransVer() {
            var temp = new string[x * y];
            var tempf = new string[x * y];
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    temp[i * y + j] = data[i * y + ((y - 1) - j)];
                    tempf[i * y + j] = floor[i * y + ((y - 1) - j)];
                }
            }
            data = temp;
            floor = tempf;
        }
    }
}