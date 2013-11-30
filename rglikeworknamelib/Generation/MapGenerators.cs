using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Generation {
    public struct MinMax {
        public Vector2 Max;
        public Vector2 Min;

        public MinMax(float x, float y, float a, float b) {
            Min = new Vector2(x, y);
            Max = new Vector2(a, b);
        }

        public MinMax(Vector2 min, Vector2 max) {
            Min = min;
            Max = max;
        }

        public float Length {
            get { return Vector2.Distance(Min, Max); }
        }
    }

    public static class MapGenerators {
        public static int seed = 12345;

        public static void FillFromTo(GameLevel ms, Vector2 from, Vector2 to, string id) {
            if (from.X > to.X) {
                float a = from.X;
                from.X = to.X;
                to.X = a;
            }
            if (from.Y > to.Y) {
                float a = from.Y;
                from.Y = to.Y;
                to.Y = a;
            }

            for (var i = (int) from.X; i <= to.X; i++) {
                for (var j = (int) from.Y; j <= to.Y; j++) {
                    ms.SetFloorSync(i, j, id);
                }
            }
        }

        public static void FillTest1(MapSector gl, string id) {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    gl.SetFloor(i, j, id);
                }
            }
        }

        public static Schemes AlterCheme(Schemes scheme, Random rnd) {
            int rr = rnd.Next(0, 4);
            int rrr = rnd.Next(0, 2);
            int rrrr = rnd.Next(0, 2);
            if (rr > 0) {
                scheme.Rotate(rr);
            }
            if (rrr > 0) {
                scheme.TransHor();
            }
            if (rrrr > 0) {
                scheme.TransVer();
            }

            return scheme;
        }

        public static List<StorageBlock> PlaceScheme(GameLevel gl, Schemes scheme, int x, int y, Random rand) {
            var l = new List<StorageBlock>();
            SectorBiom sb = SectorBiom.House;
                        switch (scheme.type) {
                            case SchemesType.Shop:
                                sb = SectorBiom.Shop;
                                break;
                            case SchemesType.Hospital:
                                sb = SectorBiom.Hospital;
                                break;
                            case SchemesType.Storage:
                                sb = SectorBiom.Storage;
                                break;
                            case SchemesType.WearShop:
                                sb = SectorBiom.WearStore;
                                break;
                        }
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    if (scheme.data[i*scheme.y + j] != "0") {
                        gl.SetBlockSync(x + i, y + j, scheme.data[i*scheme.y + j]);
                        
                        gl.SetBiomAtBlock(x + i, y + j, sb);

                        if (BlockDataBase.Data[scheme.data[i*scheme.y + j]].Prototype == typeof (StorageBlock)) {
                            List<DropGroup> a;
                            if (ItemDataBase.SpawnLists.TryGetValue(scheme.data[i * scheme.y + j], out a))
                            {
                                foreach (var drop in a)
                                {
                                    var thr = rand.Next(100) + 1;
                                    if (drop.Prob >= thr)
                                    {
                                        for (int k = 0; k < drop.Repeat; k++) {
                                            var item = ItemFactory.GetInstance(drop.Ids[rand.Next(drop.Ids.Length)],
                                                                            rand.Next(drop.MaxCount - drop.MinCount) +
                                                                            drop.MinCount);
                                            if (item != null) {
                                                ((StorageBlock) gl.GetBlock(x + i, y + j)).StoredItems.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return l;
        }

        /// <summary>
        /// </summary>
        /// <param name="mapSector">sector from where offset starts</param>
        /// <param name="schemeType"></param>
        /// <param name="posX">offset x</param>
        /// <param name="posY">offser y</param>
        /// <param name="rnd">seeded random</param>
        /// <returns>Size of placed scheme</returns>
        internal static Point PlaceRandomSchemeByType(GameLevel gl, SchemesType schemeType, int posX, int posY,
                                                      Random rnd) {
            List<Schemes> a;
            switch (schemeType) {
                case SchemesType.House:
                    a = SchemesDataBase.Houses;
                    break;

                default:
                    a = SchemesDataBase.Data.Where(x => x.type == schemeType).ToList();
                    break;
            }

            if (a.Count > 0) {
                int r = rnd.Next(0, a.Count);
                int rr = rnd.Next(0, 4);
                int rrr = rnd.Next(0, 2);
                int rrrr = rnd.Next(0, 2);
                Schemes scheme = a[r];
                if (rr > 0) {
                    scheme.Rotate(rr);
                }
                if (rrr > 0) {
                    scheme.TransHor();
                }
                if (rrrr > 0) {
                    scheme.TransVer();
                }
                string[] aa = GetInnerFloorArrayWithId(scheme, "conk_base");
                FillFloorFromArrayAndOffset(gl, scheme.x, scheme.y, aa, posX, posY);
                ClearBlocksFromArrayAndOffset(gl, scheme.x, scheme.y, aa, posX, posX);
                PlaceScheme(gl, scheme, posX, posY, rnd);
                return new Point(scheme.x, scheme.y);
            }
            return Point.Zero;
        }

        public static void ClearBlocks(MapSector gameLevel) {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Rx; j++) {
                    gameLevel.SetBlock(i, j, "0");
                }
            }
        }

        public static string[] GetInnerFloorArrayWithId(Schemes schema, string id) {
            var visited = new int[schema.data.Length];
            var todo = new Queue<int>();
            if (schema.data[0] == "0") {
                todo.Enqueue(0); // угол 0,0
            }
            if (schema.data[(schema.x - 1)*schema.y] == "0") {
                todo.Enqueue((schema.x - 1)*schema.y); //угол х,0
            }
            if (schema.data[schema.y - 1] == "0") {
                todo.Enqueue(schema.y - 1); //угол 0,у
            }
            if (schema.data[schema.y*schema.x - 1] == "0") {
                todo.Enqueue(schema.y*schema.x - 1); //угол х,у
            }

            while (todo.Count > 0) {
                int t = todo.Dequeue();
                if (t >= 0 && t <= schema.y*schema.x - 1) {
                    if (visited[t] == 0 && schema.data[t] == "0") {
                        visited[t] = 1;

                        if (t%schema.y < schema.x - 1) {
                            todo.Enqueue(t + 1);
                        }
                        if (t%schema.y > 0) {
                            todo.Enqueue(t - 1);
                        }
                        if (t/schema.y < schema.y - 1) {
                            todo.Enqueue(t + schema.y);
                        }
                        if (t/schema.y > 0) {
                            todo.Enqueue(t - schema.y);
                        }
                    }
                }
            }
            string[] converted = visited.Select(x => x == 0 ? id : "0").ToArray();
            return converted;
        }

        public static void FillFloorFromArrayAndOffset(GameLevel gl, int x, int y, string[] arr, int sx, int sy) {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (arr[i*y + j] != "0") {
                        gl.SetFloorSync(sx + i, sy + j, arr[i*y + j]);
                    }
                }
            }
        }

        public static void ClearBlocksFromArrayAndOffset(GameLevel gl, int x, int y, string[] arr, int sx, int sy) {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (arr[i*y + j] != "0") {
                        gl.SetBlockSync(sx + i, sy + j, "0");
                    }
                }
            }
        }

        public static void PlaceRoad(GameLevel gl, int startx, int starty, int sizex, int sizey, int width,
                                     bool clear = false) {
            for (int i = startx - width; i < startx + sizex + width; i++) {
                for (int j = starty - width; j < starty + sizey + width; j++) {
                    gl.SetFloorSync(i, j, "asfalt");
                    gl.SetBiomAtBlock(i, j, SectorBiom.RoadHevt);
                    if (clear) {
                        gl.SetBlockSync(i, j, "0");
                    }
                }
            }

            for (int i = startx; i < startx + sizex; i++) {
                for (int j = starty; j < starty + sizey; j++) {
                    gl.SetFloorSync(i, j, "asfalt_br");
                }
            }
        }

        internal static double Noise2D(double x, double y) {
            return ((0x6C078965*(seed ^ (((int) x*2971902361) ^ ((int) y*3572953751)))) & 0x7FFFFFFF)/
                   (double) int.MaxValue;
        }

        private static double Noise2D_2(double x, double y) {
            var n = (int) (x + y*57);
            n = (n << 13) ^ n;
            double value = (1.0f - ((n*(n*n*15731 + 789221) + 1376312589) & 0x7fffffff)/1073741824.0f);
            return Math.Abs(value);
        }

        private static double SmoothedNoise2D(double x, double y) {
            double corners = (Noise2D(x - 1, y - 1) + Noise2D(x + 1, y - 1) + Noise2D(x - 1, y + 1) +
                              Noise2D(x + 1, y + 1))/16;
            double sides = (Noise2D(x - 1, y) + Noise2D(x + 1, y) + Noise2D(x, y - 1) + Noise2D(x, y + 1))/8;
            double center = Noise2D(x, y)/4;
            return corners + sides + center;
        }

////////////////////////////////////////////////////////////////////////////


        private static double Cosine_Interpolate(double a, double b, double x) {
            double ft = x*3.141596;
            double f = (1 - Math.Cos(ft))*.5;

            return a*(1 - f) + b*f;
        }

        private static double CompileSmoothedNoise(double x, double y) {
            double int_X = (int) x;
            double fractional_X = x - int_X;

            double int_Y = (int) y;
            double fractional_Y = y - int_Y;

            double v1 = SmoothedNoise2D(int_X, int_Y);
            double v2 = SmoothedNoise2D(int_X + 1, int_Y);
            double v3 = SmoothedNoise2D(int_X, int_Y + 1);
            double v4 = SmoothedNoise2D(int_X + 1, int_Y + 1);

            double i1 = Cosine_Interpolate(v1, v2, fractional_X);
            double i2 = Cosine_Interpolate(v3, v4, fractional_X);


            return Cosine_Interpolate(i1, i2, fractional_Y);
        }

        private static double CompileSmoothedNoiseDiamond(double x, double y) {
            double int_X = (int) x;
            double fractional_X = x - int_X;

            double int_Y = (int) y;
            double fractional_Y = y - int_Y;

            double v2 = SmoothedNoise2D(int_X + 1, int_Y);
            double v3 = SmoothedNoise2D(int_X - 1, int_Y);
            double v4 = SmoothedNoise2D(int_X, int_Y + 1);
            double v1 = SmoothedNoise2D(int_X, int_Y - 1);

            double i1 = Cosine_Interpolate(v1, v2, fractional_X);
            double i2 = Cosine_Interpolate(v3, v4, fractional_X);


            return Cosine_Interpolate(i1, i2, fractional_Y);
        }

        private static double CompileNoise(double x, double y) {
            double int_X = (int) x;
            double fractional_X = x - int_X;

            double int_Y = (int) y;
            double fractional_Y = y - int_Y;

            double v1 = Noise2D(int_X, int_Y);
            double v2 = Noise2D(int_X + 1, int_Y);
            double v3 = Noise2D(int_X, int_Y + 1);
            double v4 = Noise2D(int_X + 1, int_Y + 1);

            double i1 = Cosine_Interpolate(v1, v2, fractional_X);
            double i2 = Cosine_Interpolate(v3, v4, fractional_X);


            return Cosine_Interpolate(i1, i2, fractional_Y);
        }

        public static double[] NoiseMap(double x, double y, int sx, int sy, double zoom = 1) {
            var a = new double[sx*sy];
            const int oct = 5;

            for (int i = 0; i < sx; i++) {
                for (int j = 0; j < sy; j++) {
                    double tt = 0;
                    double diver = 1;
                    for (int w = 1; w <= oct; w++) {
                        tt += CompileNoise((i + x*sx)*zoom/diver, (j + y*sy)*zoom/diver);
                        diver *= 2;
                    }
                    a[i*sy + j] = tt/oct;
                }
            }
            return a;
        }

        public static double[] CompiledNoiseMap(double x, double y, int sx, int sy, double zoom = 1) {
            var a = new double[sx*sy];
            const int oct = 5;

            for (int i = 0; i < sx; i++) {
                for (int j = 0; j < sy; j++) {
                    double tt = 0;
                    double diver = 1;
                    for (int w = 1; w <= oct; w++) {
                        tt += CompileSmoothedNoiseDiamond((i + x*sx)*zoom/diver, (j + y*sy)*zoom/diver);
                        diver *= 2;
                    }
                    a[i*sy + j] = tt/oct;
                }
            }
            return a;
        }

        public static double[] SmoothNoiseMap(double[] a) {
            int sx, sy = sx = (int) Math.Sqrt(a.Length);
            for (int i = 0; i < sx - 1; i++) {
                for (int j = 0; j < sy - 1; j++) {
                    double round = ((a[(i)*sy + j] + a[(i + 1)*sy + j] + a[(i + 1)*sy + j + 1] + a[(i)*sy + j + 1])/4.0);
                    a[(i)*sy + j] = ((a[(i)*sy + j] + round)/2.0);
                    a[(i + 1)*sy + j] = ((a[(i + 1)*sy + j] + round)/2.0);
                    a[(i)*sy + j + 1] = ((a[(i)*sy + j + 1] + round)/2.0);
                    a[(i + 1)*sy + j + 1] = ((a[(i + 1)*sy + j + 1] + round)/2.0);
                }
            }
            return a;
        }

        public static double[] PostEffect(double[] img, int iterations, double smooth) {
            for (int i = 0; i < img.Length; i++) {
                double s = (float) img[i];
                double ds = s*iterations - ((int) (s*iterations));
                ds = smooth*(ds - 0.5f) + 0.5f;
                if (ds > 1) {
                    ds = 1;
                }
                if (ds < 0) {
                    ds = 0;
                }
                s = (((int) (s*iterations)) + ds)/iterations;
                img[i] = s;
            }
            return img;
        }

        public static void FloorPerlin(MapSector mapSector) {
            //double[] a = NoiseMap(mapSector.SectorOffsetX, mapSector.SectorOffsetY, MapSector.Rx, MapSector.Ry, 0.4);
            double[] a =
                PostEffect(
                    SmoothNoiseMap(
                        SmoothNoiseMap(NoiseMap(mapSector.SectorOffsetX, mapSector.SectorOffsetY, MapSector.Rx,
                                                MapSector.Ry, 0.05))), 5, 2.5);
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    double t = a[i*MapSector.Ry + j];
                    if (t > 0.7) {
                        mapSector.SetFloor(i, j, "1");
                    }
                    else {
                        if (t > 0.5) {
                            mapSector.SetFloor(i, j, "1");
                        }
                        else {
                            mapSector.SetFloor(i, j, "grass_base");
                        }
                    }
                }
            }
        }

        public static void GenerateCity(GameLevel gl, Random rnd, int mainW, int mainH, int posX, int posY) {
            List<Rectangle> t = GenerateRoads(gl, rnd, mainW, mainH, posX, posY, 10);

            foreach (Rectangle rect in t) {
                int ostx = rect.Width - 14;
                int osty = rect.Height - 14;

                var off = new Point();
                int maxx = 0;
                for (;;) {
                    Schemes ch = SchemesDataBase.NormalCity[rnd.Next(SchemesDataBase.NormalCity.Count)];
                    if (off.X + ch.x > ostx) {
                        break;
                    }
                    ch = AlterCheme(ch, rnd);
                    string[] aa = GetInnerFloorArrayWithId(ch, "conk_base");
                    FillFloorFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    ClearBlocksFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    PlaceScheme(gl, ch, rect.X + off.X + 7, rect.Y + off.Y + 7, rnd);
                    off.X += ch.x;
                    if (maxx < ch.x) {
                        maxx = ch.y;
                    }
                    if (off.X > ostx) {
                        break;
                    }
                }

                off = new Point(0, maxx);
                for (;;) {
                    Schemes ch = SchemesDataBase.Data[rnd.Next(SchemesDataBase.Data.Count)];
                    if (off.Y + ch.y > osty) {
                        break;
                    }
                    ch = AlterCheme(ch, rnd);
                    string[] aa = GetInnerFloorArrayWithId(ch, "conk_base");
                    FillFloorFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    ClearBlocksFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    PlaceScheme(gl, ch, rect.X + off.X + 7, rect.Y + off.Y + 7, rnd);
                    off.Y += ch.y;
                }
            }

            foreach (Rectangle rect in t) {
                PlaceRoad(gl, rect.X, rect.Y, 0, rect.Height, 6, true);
                PlaceRoad(gl, rect.X, rect.Y, rect.Width, 0, 6, true);
                PlaceRoad(gl, rect.X + rect.Width, rect.Y, 0, rect.Height, 6, true);
                PlaceRoad(gl, rect.X, rect.Y + rect.Height, rect.Width, 0, 6, true);
            }
        }

        public static List<Rectangle> GenerateRoads(GameLevel gl, Random rnd, int mainW, int mainH, int posX, int posY,
                                                    int splitter) {
            var t = new List<Rectangle>();
            t.Add(new Rectangle(posX, posY, mainW, mainH));

            //major road
            //PlaceRoad(gl, posX, posY, posX + mainW, posY, 6, true);
            //PlaceRoad(gl, posX, posY + mainH, posX + mainW, posY + mainH, 6, true);
            //PlaceRoad(gl, posX, posY, posX, posY + mainH, 6, true);
            //PlaceRoad(gl, posX + mainW, posY, posX + mainW, posY + mainH, 6, true);

            for (int i = 0; i < splitter; i++) {
                List<int> A = t.Select(x => x.Height*x.Width).ToList();
                int max = A.IndexOf(A.Max());
                Rectangle a = t[max];

                t.Remove(a);
                int type = rnd.Next(0, 234234);
                int middle = 0;
                if (a.Width > a.Height) {
                    middle = rnd.Next(a.Width/3, (a.Width/3)*2);
                    t.Add(new Rectangle(a.X, a.Y, middle, a.Height));
                    t.Add(new Rectangle(a.X + middle, a.Y, a.Width - middle, a.Height));
                }
                else {
                    middle = rnd.Next(a.Height/3, (a.Height/3)*2);
                    t.Add(new Rectangle(a.X, a.Y, a.Width, middle));
                    t.Add(new Rectangle(a.X, a.Y + middle, a.Width, a.Height - middle));
                }
            }

            return t;
        }

        public static string GetMost(int offX, int offY) {
            double[] a =
                PostEffect(
                    SmoothNoiseMap(
                        SmoothNoiseMap(NoiseMap(offX, offY, MapSector.Rx,
                                                MapSector.Ry, 0.05))), 5, 2.5);
            int grass_count = 0;
            int dirt_ground = 0;
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    double t = a[i*MapSector.Ry + j];
                    if (t > 0.7) {
                        dirt_ground++;
                    }
                    else {
                        if (t > 0.5) {
                            dirt_ground++;
                        }
                        else {
                            grass_count++;
                        }
                    }
                }
            }

            return grass_count > dirt_ground ? "grass_base" : "1";
        }

        public static void ClearBlocksFromTo(GameLevel ms, Vector2 from, Vector2 to) {
            if (from.X > to.X) {
                float a = from.X;
                from.X = to.X;
                to.X = a;
            }
            if (from.Y > to.Y) {
                float a = from.Y;
                from.Y = to.Y;
                to.Y = a;
            }

            for (var i = (int) from.X; i <= to.X; i++) {
                for (var j = (int) from.Y; j <= to.Y; j++) {
                    ms.SetBlockSync(i, j, "0");
                }
            }
        }
    }

    public class MathGenerators {
        private static double fade(double t) {
            return t*t*t*(t*(t*6 - 15) + 10);
        }

        private static double lerp(double t, double a, double b) {
            return a + t*(b - a);
        }

        private static double grad(int hash, double x, double y, double z) {
            int h = hash & 15;
            double u = h < 8 ? x : y,
                   v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}