using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using System.Linq;

namespace rglikeworknamelib.Generation
{

    public struct MinMax {

        public MinMax(float x, float y, float a, float b)
        {
            Min = new Vector2(x,y);
            Max = new Vector2(a, b);
        }

        public MinMax(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public Vector2 Min, Max;
        public float Length {
            get { return Vector2.Distance(Min, Max); }
        }
    }

    public static class MapGenerators
    {
        public static void FillFromTo(MapSector ms, Vector2 from, Vector2 to, string id) {
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

            var i1 = ms.SectorOffsetX * MapSector.Rx;
            var i2 = ms.SectorOffsetY * MapSector.Ry;

            for (int i = (int)from.X; i <= to.X; i++) {
                for (int j = (int)from.Y; j <= to.Y; j++) {
                    ms.Parent.SetFloor(i+i1, j+i2, id);
                }
            }
        }

        public static void FillTest1(MapSector gl, string id)
        {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    gl.SetFloor(i, j, id);
                }
            }
        }

        internal static void PlaceScheme(GameLevel gl, Schemes scheme, int x, int y)
        {
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                        if (scheme.data[i * scheme.y + j] != "0") {
                            gl.SetBlockSync(x + i, y + j, scheme.data[i * scheme.y + j]);
                        }
                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapSector">sector from where offset starts</param>
        /// <param name="schemeType"></param>
        /// <param name="posX">offset x</param>
        /// <param name="posY">offser y</param>
        /// <param name="rnd">seeded random</param>
        /// <returns>Size of placed scheme</returns>
        internal static Point PlaceRandomSchemeByType(GameLevel gl, SchemesType schemeType, int posX, int posY, Random rnd)
        {
            List<Schemes> a;
            switch (schemeType) {
                case SchemesType.House:
                    a = SchemesDataBase.Houses;
                    break;

                default:
                    a = SchemesDataBase.Data.Where(x => x.type == schemeType).ToList();
                    break;
            }

            if(a.Count > 0) {
                int r = rnd.Next(0, a.Count);
                int rr = rnd.Next(0, 4);
                int rrr = rnd.Next(0, 2);
                int rrrr = rnd.Next(0, 2);
                var scheme = a[r];
                if (rr > 0) {
                    scheme.Rotate(rr);
                }
                if(rrr > 0) {
                    scheme.TransHor();
                }
                if (rrrr > 0)
                {
                    scheme.TransVer();
                }
                var aa = GetInnerFloorArrayWithId(scheme, "conk_base");
                FillFloorFromArrayAndOffset(gl, scheme.x, scheme.y, aa, posX, posY);
                ClearBlocksFromArrayAndOffset(gl, scheme.x, scheme.y, aa, posX, posX);
                PlaceScheme(gl, scheme, posX, posY);
                return new Point(scheme.x, scheme.y);
            }
            return Point.Zero;
        }

        public static void ClearBlocks(MapSector gameLevel) {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Rx; j++) { 
                    gameLevel.SetBlock(i,j,"0");
                }
            }
        }

        public static string[] GetInnerFloorArrayWithId(Schemes schema, string id) {

            int[] visited = new int[schema.data.Length];
            Queue<int> todo = new Queue<int>();
            if (schema.data[0] == "0") todo.Enqueue(0); // угол 0,0
            if (schema.data[(schema.x - 1) * schema.y] == "0") todo.Enqueue((schema.x - 1) * schema.y); //угол х,0
            if (schema.data[schema.y - 1] == "0") todo.Enqueue(schema.y - 1); //угол 0,у
            if (schema.data[schema.y * schema.x - 1] == "0") todo.Enqueue(schema.y * schema.x - 1); //угол х,у

            while (todo.Count > 0) {
                int t = todo.Dequeue();
                if (t >= 0 && t <= schema.y * schema.x - 1) {
                    if (visited[t] == 0 && schema.data[t] == "0") {
                        visited[t] = 1;

                        if (t % schema.y < schema.x - 1) todo.Enqueue(t + 1);
                        if (t % schema.y > 0) todo.Enqueue(t - 1);
                        if (t / schema.y < schema.y - 1) todo.Enqueue(t + schema.y);
                        if (t / schema.y > 0) todo.Enqueue(t - schema.y);
                    }
                }
            }
            string[] converted = visited.Select(x => x == 0 ? id : "0").ToArray();
            return converted;
        }

        public static void FillFloorFromArrayAndOffset(GameLevel gl, int x, int y, string[] arr, int sx, int sy) {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (arr[i * y + j] != "0" && sx + i < MapSector.Rx && sy + j < MapSector.Ry) {
                        
                        gl.SetFloorSync(sx + i, sy + j, arr[i * y + j]);
                    }
                }
            }
        }

        public static void ClearBlocksFromArrayAndOffset(GameLevel gl, int x, int y, string[] arr, int sx, int sy)
        {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (arr[i * y + j] != "0" && sx + i < MapSector.Rx && sy + j < MapSector.Ry)
                    {

                        gl.SetBlockSync(sx + i, sy + j, "0");
                    }
                }
            }
        }

        public static int seed = 12345;

        internal static double Noise2D(double x, double y) {
            return ((0x6C078965*(seed ^ (((int) x*2971902361) ^ ((int) y*3572953751)))) & 0x7FFFFFFF)/(double)int.MaxValue;
        }

        private static double Noise2D_2(double x, double y) {
            int n = (int) (x + y*57);
            n = (n << 13) ^ n;
            double value = (1.0f - ((n*(n*n*15731 + 789221) + 1376312589) & 0x7fffffff)/1073741824.0f);
            return Math.Abs(value);
        }

        private static double SmoothedNoise2D(double x, double y) {
            double corners = (Noise2D(x - 1, y - 1) + Noise2D(x + 1, y - 1) + Noise2D(x - 1, y + 1) + Noise2D(x + 1, y + 1))/16;
            double sides = (Noise2D(x - 1, y) + Noise2D(x + 1, y) + Noise2D(x, y - 1) + Noise2D(x, y + 1))/8;
            double center = Noise2D(x, y)/4;
            return corners + sides + center;
        }

////////////////////////////////////////////////////////////////////////////


        private static double Cosine_Interpolate(double a, double b, double x) {
            var ft = x*3.141596;
            var f = (1 - Math.Cos(ft))*.5;

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

        private static double CompileSmoothedNoiseDiamond(double x, double y)
        {
            double int_X = (int)x;
            double fractional_X = x - int_X;

            double int_Y = (int)y;
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
            double[] a = new double[sx*sy];
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

        public static double[] CompiledNoiseMap(double x, double y, int sx, int sy, double zoom = 1)
        {
            double[] a = new double[sx * sy];
            const int oct = 5;

            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    double tt = 0;
                    double diver = 1;
                    for (int w = 1; w <= oct; w++)
                    {
                        tt += CompileSmoothedNoiseDiamond((i + x * sx) * zoom / diver, (j + y * sy) * zoom / diver);
                        diver *= 2;
                    }
                    a[i * sy + j] = tt / oct;
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

        public static void FloorPerlin(MapSector mapSector)
        {
            //double[] a = NoiseMap(mapSector.SectorOffsetX, mapSector.SectorOffsetY, MapSector.Rx, MapSector.Ry, 0.4);
            double[] a =
                PostEffect(
                    SmoothNoiseMap(
                        SmoothNoiseMap(NoiseMap(mapSector.SectorOffsetX, mapSector.SectorOffsetY, MapSector.Rx,
                                                MapSector.Ry, 0.05))), 5, 2.5);
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    double t = a[i * MapSector.Ry + j];
                    if (t > 0.7) {
                        mapSector.SetFloor(i, j, "1");
                    } else {
                        if (t > 0.5) {
                            mapSector.SetFloor(i, j, "1");
                        } else {
                            mapSector.SetFloor(i, j, "grass_base");
                        }
                    }
                }
            }
        }

        public static HashSet<Tuple<int, int>> GenerateRoadmap(int seed) {
            var rand = new Random(seed);
            HashSet<Tuple<int, int>> set = new HashSet<Tuple<int, int>>();
            Tuple<int, int> temp = new Tuple<int, int>(0,0);
            int o = 0;
            while (o < 10000)
            {
                
                set.Add(temp);
                var adder = rand.Next(1, 5);
                int addx = 0, addy = 0;
                switch (adder) {
                    case 1:
                        addx = -1;
                        break;
                    case 2:
                        addx = 1;
                        break;
                    case 3:
                        addy = -1;
                        break;
                    case 4:
                        addy = 1;
                        break;
                }
                var len = rand.Next(1, 20);
                    for (int j = 0; j < len; j++) {
                        temp = new Tuple<int, int>(temp.Item1 + addx, temp.Item2 + addy);
                        set.Add(new Tuple<int, int>(temp.Item1, temp.Item2));
                    }

                    var newroad = rand.Next(1, 10);
                    if (newroad == 1) {
                        temp = new Tuple<int, int>(rand.Next(-1000, 1000), rand.Next(-1000, 1000));
                    }
                    o++;
            }
            return set;
        } 

        public static string GetMost(int offX, int offY)
        {
            double[] a =
    PostEffect(
        SmoothNoiseMap(
            SmoothNoiseMap(NoiseMap(offX, offY, MapSector.Rx,
                                    MapSector.Ry, 0.05))), 5, 2.5);
            int grass_count = 0;
            int dirt_ground = 0;
            for (int i = 0; i < MapSector.Rx; i++)
            {
                for (int j = 0; j < MapSector.Ry; j++)
                {
                    double t = a[i * MapSector.Ry + j];
                    if (t > 0.7)
                    {
                        dirt_ground++;
                    }
                    else
                    {
                        if (t > 0.5)
                        {
                            dirt_ground++;
                        }
                        else
                        {
                            grass_count++;
                        }
                    }
                }
            }

            return grass_count > dirt_ground ? "grass_base" : "1";
        }


        public static void GenerateRoad(GameLevel ms, SectorBiom rnd) {
            switch (rnd) {
                case SectorBiom.RoadCross:
                    FillFloorFromArrayAndOffset(ms, 16, 16, shosse_cross, 0, 0);
                    break;
                case SectorBiom.RoadHevt:
                    FillFloorFromArrayAndOffset(ms, 16, 16, shosse_vertical, 0, 0);
                    break;
                case SectorBiom.RoadHor:
                    FillFloorFromArrayAndOffset(ms, 16, 16, shosse_hor, 0, 0);
                    break;
            }
        }

        private static string[] shosse_vertical = new[] {
                                                            "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                                                            "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                                                            "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                            "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br"
                                                            , "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br",
                                                            "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br"
                                                            , "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br",
                                                            "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br", "asfalt_br"
                                                            , "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt"
                                                            , "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                            "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0", "0", "0",
                                                            "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                                                            "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"
                                                        };

        private static string[] shosse_hor = new[] {
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0",
                                                       "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt_br",
                                                       "asfalt_br", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0"
                                                   };

        private static string[] shosse_cross = new[] {
                                                         "0", "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0", "0"
                                                         , "0", "asfalt", "asfalt", "asfalt", "asfalt", "asfalt",
                                                         "asfalt_br", "asfalt_br", "asfalt", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "0", "0", "0", "0", "asfalt", "asfalt",
                                                         "asfalt", "asfalt", "asfalt", "asfalt_br", "asfalt_br",
                                                         "asfalt", "asfalt", "asfalt", "asfalt", "asfalt", "0", "0"
                                                     };
    }

   public class MathGenerators {
   static double fade(double t) { return t * t * t * (t * (t * 6 - 15) + 10); }
   static double lerp(double t, double a, double b) { return a + t * (b - a); }
   static double grad(int hash, double x, double y, double z) {
      int h = hash & 15;                      
      double u = h<8 ? x : y,                 
             v = h<4 ? y : h==12||h==14 ? x : z;
      return ((h&1) == 0 ? u : -u) + ((h&2) == 0 ? v : -v);
   }
}
}
