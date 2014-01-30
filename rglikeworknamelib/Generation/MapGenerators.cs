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
        public static int Seed;

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

        public static List<Block> PlaceScheme(GameLevel gl, Schemes scheme, int x, int y, Random rand) {
            var l = new List<Block>();
            SectorBiom sb = SectorBiom.House;
                        switch (scheme.type) {
                            case SectorBiom.Shop:
                                sb = SectorBiom.Shop;
                                break;
                            case SectorBiom.Hospital:
                                sb = SectorBiom.Hospital;
                                break;
                            case SectorBiom.Storage:
                                sb = SectorBiom.Storage;
                                break;
                            case SectorBiom.WearShop:
                                sb = SectorBiom.WearStore;
                                break;
                        }
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    string blockId = scheme.data[i*scheme.y + j];
                        var block = gl.SetBlockSyncAndReturn(x + i, y + j, blockId);
                        //var floor = gl.GetFloorSync(x + i, y + j);
                        //if (floor.Id == "conk_base" && !block.Data.Wallmaker)
                        //{
                        //block.Inner = true;
                        //}
                        if (BlockDataBase.Data[blockId].SmartAction == SmartAction.ActionOpenContainer) {
                            if (block.Data.ItemSpawn != null)
                            {
                                foreach (var drop in block.Data.ItemSpawn)
                                {
                                    var thr = rand.Next(100) + 1;
                                    if (drop.Prob >= thr)
                                    {
                                        for (int k = 0; k < drop.Repeat; k++) {
                                            var item = ItemFactory.GetInstance(drop.Ids[rand.Next(drop.Ids.Count)],
                                                                            rand.Next(drop.Max - drop.Min) +
                                                                            drop.Min);
                                            if (item != null) {
                                                block.StoredItems.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        
                    }
                }
            }

            //inner light part
            //bool inner = false;
            //for (int i = 0; i < scheme.x; i++) {
            //    for (int j = 0; j < scheme.y; j++)
            //    {
            //        var block = gl.GetBlockSync(x + i, y + 0 + j);
            //        block.Inner = inner;

            //        if (block.Id == "1") {
            //            inner = !inner;
            //        }
            //    }
            //}
            //inner = false;
            //for (int j = 0; j < scheme.y; j++)
            //{
            //    for (int i = 0; i < scheme.x; i++)
            //    {
            //        var block = gl.GetBlockSync(x + i, y + 0 + j);
            //        block.Inner = inner;

            //        if (block.Id == "1")
            //        {
            //            inner = !inner;
            //        }
            //    }
            //}
            gl.SetBiomAtBlock(x, y, sb);
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
        internal static Point PlaceRandomSchemeByType(GameLevel gl, SectorBiom schemeType, int posX, int posY,
                                                      Random rnd) {
            List<Schemes> a;
            switch (schemeType) {
                case SectorBiom.House:
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
                    
                }
            }
            gl.SetBiomAtBlock(startx, starty, SectorBiom.RoadHevt);
            if (clear) {
                for (int i = startx - width; i < startx + sizex + width; i++) {
                    for (int j = starty - width; j < starty + sizey + width; j++) {
                        gl.SetBlockSync(i, j, "0");
                    }
                }
            }

            //for (int i = startx; i < startx + sizex; i++) {
            //    for (int j = starty; j < starty + sizey; j++) {
            //        gl.SetFloorSync(i, j, "asfalt_br");
            //    }
            //}
        }

        internal static double Noise2D(double x, double y) {
            return ((0x6C078965*(Seed ^ (((int) x*2971902361) ^ ((int) y*3572953751)))) & 0x7FFFFFFF)/
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

            int i = 1;
            foreach (Rectangle rect in t) {
                Settings.NTS1 = "Map generation :";
                Settings.NTS2 = string.Format("{0}/{1} ({2})", i, t.Count, gl.megaMap.Count);
                Settings.NeedToShowInfoWindow = true;
                i++;

                int ostx = rect.Width - 14;
                int osty = rect.Height - 14;

                var off = new Point();
                int maxx = 0;
                while (true)
                {
                    Schemes ch = SchemesDataBase.NormalCity[rnd.Next(SchemesDataBase.NormalCity.Count)];
                    if (off.X + ch.x > ostx) {
                        break;
                    }
                    ch = AlterCheme(ch, rnd);
                    string[] aa = GetInnerFloorArrayWithId(ch, "conk_base");
                    FillFloorFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    //ClearBlocksFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
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
                while (true)
                {
                    Schemes ch = SchemesDataBase.Data[rnd.Next(SchemesDataBase.Data.Count)];
                    if (off.Y + ch.y > osty) {
                        break;
                    }
                    ch = AlterCheme(ch, rnd);
                    string[] aa = GetInnerFloorArrayWithId(ch, "conk_base");
                    FillFloorFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    //ClearBlocksFromArrayAndOffset(gl, ch.x, ch.y, aa, rect.X + off.X + 7, rect.Y + off.Y + 7);
                    PlaceScheme(gl, ch, rect.X + off.X + 7, rect.Y + off.Y + 7, rnd);
                    off.Y += ch.y;
                }
            }

            foreach (Rectangle rect in t) {
                PlaceRoad(gl, rect.X, rect.Y, 0, rect.Height, 6, true);
                PlaceRoad(gl, rect.X, rect.Y, rect.Width, 0, 6, true);
                PlaceRoad(gl, rect.X + rect.Width, rect.Y, 0, rect.Height, 6, true);
                PlaceRoad(gl, rect.X, rect.Y + rect.Height, rect.Width, 0, 6, true);

                for (int n = -1; n < 3; n++) {
                    for (int k = -1; k < 3; k++) {
                        var sector = gl.GetSectorSync(rect.X / 16 + n, rect.Y / 16 + k);
                        int rnd1 = rnd.Next(0, 5);
                        int rnd2 = rnd.Next(0, 3);
                        for (int j = 1; j < rnd1; j++) {
                            sector.Spawn("zombie1", rnd);
                        }
                        for (int j = 1; j < rnd2; j++) {
                            sector.Spawn("hdzombie", rnd);
                        }
                    }
                }
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

    public static class MapGenerators2 {
        public static List<KeyValuePair<Point, MapSector>> GenerateCityAt(GameLevel gl, Random rnd, int x, int y) {
            var temp = new List<KeyValuePair<Point, MapSector>>();

            var rects = GenerateRoads(rnd, 10, 10, x, y, 8);
            var already = new List<Point>();

            foreach (var rect in rects) {
                for (int i = rect.Top; i < rect.Bottom; i++) {
                    PutRoadVert(gl, rect.Left, i, already, temp);
                    PutRoadVert(gl, rect.Right, i, already, temp);
                    if (rnd.Next(3) != 0) {
                        PutHouse(gl, rect.Left - 1, i, temp, already, rnd);
                    }
                    if (rnd.Next(3) != 0) {
                        PutHouse(gl, rect.Right + 1, i, temp, already, rnd);
                    }
                }
                for (int i = rect.Left; i < rect.Right; i++)
                {
                    PutRoadHor(gl, i, rect.Top, already, temp);
                    PutRoadHor(gl, i, rect.Bottom, already, temp);
                    if (rnd.Next(3) != 0) {
                        PutHouse(gl, i, rect.Top - 1, temp, already, rnd);
                    }
                    if (rnd.Next(3) != 0) {
                        PutHouse(gl, i, rect.Bottom + 1, temp, already, rnd);
                    }
                }
                
            }

            return temp;
        }

        private static void PutHouse(GameLevel gl, int i, int j, List<KeyValuePair<Point, MapSector>> temp, List<Point> already, Random rnd) {
            var sch = SchemesDataBase.NormalCity[rnd.Next(SchemesDataBase.NormalCity.Count)];
            var a = MapGenerators.AlterCheme(sch, rnd);

            var p = new Point(i, j);

            if(already.Contains(p)){return;}

            already.Add(p);
            var sector = new MapSector(gl, i, j);
            sector.Rebuild(gl.MapSeed);

            for (int k = 0; k < MapSector.Rx; k++) {
                for (int l = 0; l < MapSector.Ry; l++) {
                    if (k < a.x && l < a.y) {
                        var tt = a.floor[k * a.y + l];
                        bool hasFloor = false;
                        if (tt != "0")
                        {
                            sector.SetFloor(k, l, tt);
                            hasFloor = true;
                        }
                        tt = a.data[k*a.y + l];
                        if (tt != "0" || hasFloor) {
                            sector.SetBlock(k, l, tt);
                            var block = sector.GetBlock(k, l);

                            BlockDataBase.TrySpawnItems(rnd, block);
                            ItemDataBase.StackSimilar(ref block.StoredItems);
                        }
                    }
                }
            }
            sector.Biom = a.type;

            temp.Add(new KeyValuePair<Point, MapSector>(p, sector));
        }

        private static void PutRoadVert(GameLevel gl, int i, int j, List<Point> already, List<KeyValuePair<Point, MapSector>> temp) {
            var p = new Point(i, j);
            if (!already.Contains(p)) {
                already.Add(p);
                var sector = new MapSector(gl, p.X, p.Y);
                sector.Rebuild(gl.MapSeed);
                sector.Biom = SectorBiom.Road;
                for (int k = 0; k < MapSector.Ry; k++) {
                    for (int n = MapSector.Rx / 2 - 5; n < MapSector.Rx / 2 + 5; n++)
                    {
                        sector.SetFloor(n, k, "asfalt");
                        sector.SetBlock(n, k, "0");
                    }
                    for (int n = MapSector.Rx / 2 - 5 - 3; n < MapSector.Rx / 2 - 5; n++)
                    {
                        sector.SetFloor(n, k, "conk_base");
                        sector.SetBlock(n, k, "0");
                    }
                    for (int n = MapSector.Rx / 2 + 5; n < MapSector.Rx / 2 + 5 + 3; n++)
                    {
                        sector.SetFloor(n, k, "conk_base");
                        sector.SetBlock(n, k, "0");
                    }
                }
                temp.Add(new KeyValuePair<Point, MapSector>(p, sector));
            }
        }

        private static void PutRoadHor(GameLevel gl, int i, int j, List<Point> already, List<KeyValuePair<Point, MapSector>> temp)
        {
            var p = new Point(i, j);
            if (!already.Contains(p))
            {
                already.Add(p);
                var sector = new MapSector(gl, p.X, p.Y);
                sector.Rebuild(gl.MapSeed);
                sector.Biom = SectorBiom.Road;
                for (int n = 0; n < MapSector.Rx; n++)
                {
                    for (int k = MapSector.Rx / 2 - 5; k < MapSector.Rx / 2 + 5; k++)
                    {
                        sector.SetFloor(n, k, "asfalt");
                        sector.SetBlock(n, k, "0");
                    }
                    for (int k = MapSector.Rx / 2 - 5 - 3; k < MapSector.Rx / 2 - 5; k++)
                    {
                        sector.SetFloor(n, k, "conk_base");
                        sector.SetBlock(n, k, "0");
                    }
                    for (int k = MapSector.Rx / 2 + 5; k < MapSector.Rx / 2 + 5 + 3; k++)
                    {
                        sector.SetFloor(n, k, "conk_base");
                        sector.SetBlock(n, k, "0");
                    }
                }
                temp.Add(new KeyValuePair<Point, MapSector>(p, sector));
            }
        }

        public static List<Rectangle> GenerateRoads(Random rnd, int mainW, int mainH, int posX, int posY, int splitter)
        {
            var t = new List<Rectangle>();
            t.Add(new Rectangle(posX, posY, mainW, mainH));

            for (int i = 0; i < splitter; i++)
            {
                List<int> A = t.Select(x => x.Height * x.Width).ToList();
                int max = A.IndexOf(A.Max());
                Rectangle a = t[max];

                t.Remove(a);
                int type = rnd.Next(0, 234234);
                int middle = 0;
                if (a.Width > a.Height)
                {
                    middle = rnd.Next(a.Width / 3, (a.Width / 3) * 2);
                    t.Add(new Rectangle(a.X, a.Y, middle, a.Height));
                    t.Add(new Rectangle(a.X + middle, a.Y, a.Width - middle, a.Height));
                }
                else
                {
                    middle = rnd.Next(a.Height / 3, (a.Height / 3) * 2);
                    t.Add(new Rectangle(a.X, a.Y, a.Width, middle));
                    t.Add(new Rectangle(a.X, a.Y + middle, a.Width, a.Height - middle));
                }
            }

            return t;
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