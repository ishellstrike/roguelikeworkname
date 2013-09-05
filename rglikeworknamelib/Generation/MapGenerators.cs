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

        public MinMax(Vector2 a, Vector2 b)
        {
            Min = a;
            Max = b;
        }

        public Vector2 Min, Max;
        public float Length {
            get { return Vector2.Distance(Min, Max); }
        }
    }

    public static class MapGenerators
    {
        public static void GenerateStreetGrid(MapSector ms, Vector2[] initialNodes, Random rnd) {
            int gg = rnd.Next(1, 10);
            for (int i = 0; i < initialNodes.Length; i++) {
                if (gg < 8) {
                    if (initialNodes[i].X == MapSector.Rx - 1 || initialNodes[i].X == 0) {
                        FillFromTo(ms, new Vector2(MapSector.Rx - 1, initialNodes[i].Y - 1), new Vector2(0, initialNodes[i].Y + 1), "2");
                        ms.AddInitialNode(0, initialNodes[i].Y);
                    }
                    if (initialNodes[i].Y == MapSector.Ry - 1 || initialNodes[i].Y == 0) {
                        FillFromTo(ms, new Vector2(initialNodes[i].X - 1, MapSector.Ry - 1), new Vector2(initialNodes[i].X + 1, 0), "2");
                        ms.AddInitialNode(initialNodes[i].X, 0);
                    }          
                } else {
                    //if (initialNodes[i].X == MapSector.Rx - 1 || initialNodes[i].X == 0) {
                    //    FillFromTo(ms, new Vector2(initialNodes[i].X, initialNodes[i].Y - 1), new Vector2(MapSector.Rx/2, initialNodes[i].Y + 1), 2);
                    //}
                    //if (initialNodes[i].Y == MapSector.Ry - 1 || initialNodes[i].Y == 0) {
                    //    FillFromTo(ms, new Vector2(initialNodes[i].X - 1, initialNodes[i].Y), new Vector2(initialNodes[i].X + 1, MapSector.Ry/2), 2);
                    //}
                }
            }
            if (gg == 4) {

                    ms.AddInitialNode(MapSector.Rx - 1, rnd.Next(1, MapSector.Ry-2)); 
                    ms.AddInitialNode(0, rnd.Next(1, MapSector.Ry - 2));
                    ms.AddInitialNode(rnd.Next(1, MapSector.Rx - 2), MapSector.Ry - 1);
                    ms.AddInitialNode(rnd.Next(1, MapSector.Rx - 2), 0);
            }
        }

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

            for (int i = (int)from.X; i <= to.X; i++) {
                for (int j = (int)from.Y; j <= to.Y; j++) {
                    ms.SetFloor(i, j, id);
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

        private static int GetRandomCoordInCenter(int rx, Random rnd)
        {
            return rnd.Next(-rx/10,rx/10)+rx/2;
        }

        internal static void PlaceScheme(MapSector gl, GameLevel level, Schemes scheme, int x, int y)
        {
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                        if (scheme.data[i * scheme.y + j] != "0") {
                            level.SetBlock(x + i + gl.SectorOffsetX * MapSector.Rx, y + j + gl.SectorOffsetY * MapSector.Ry, scheme.data[i * scheme.y + j]);
                        }
                    
                }
            }
        }

        internal static void PlaceRandomSchemeByType(MapSector mapSector, SchemesType schemeType, int posX, int posY, Random rnd)
        {
            List<Schemes> a;
            switch (schemeType) {
                case SchemesType.house:
                    a = SchemesDataBase.Houses;
                    break;

                default:
                    a = SchemesDataBase.Data.Where(x => x.type == schemeType).ToList();
                    break;
            }

            if(a.Count > 0) {
                int r = rnd.Next(0, a.Count);
                var aa = GetInnerFloorArrayWithId(a[r], "conk_base");
                FillFloorFromArrayAndOffset(mapSector, mapSector.Parent, a[r].x, a[r].y, aa, posX, posY);
                ClearBlocksFromArrayAndOffset(mapSector, mapSector.Parent, a[r].x, a[r].y, aa, posX, posY);
                PlaceScheme(mapSector, mapSector.Parent, a[r], posX, posY);
            }
        }

        public static void ClearBlocks(MapSector gameLevel) {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Rx; j++) { 
                    gameLevel.SetBlock(i,j,"0");
                }
            }
        }

        public static string[] GetInnerFloorArrayWithId(Schemes a, string id) {

            int[] visited = new int[a.data.Length];
            Queue<int> todo = new Queue<int>();
            if (a.data[0] == "0") todo.Enqueue(0); // угол 0,0
            if (a.data[(a.x - 1) * a.y] == "0") todo.Enqueue((a.x - 1) * a.y); //угол х,0
            if (a.data[a.y - 1] == "0") todo.Enqueue(a.y - 1); //угол 0,у
            if (a.data[a.y * a.x - 1] == "0") todo.Enqueue(a.y * a.x - 1); //угол х,у

            while (todo.Count > 0) {
                int t = todo.Dequeue();
                if (t >= 0 && t <= a.y * a.x - 1) {
                    if (visited[t] == 0 && a.data[t] == "0") {
                        visited[t] = 1;

                        if (t % a.y < a.x - 1) todo.Enqueue(t + 1);
                        if (t % a.y > 0) todo.Enqueue(t - 1);
                        if (t / a.y < a.y - 1) todo.Enqueue(t + a.y);
                        if (t / a.y > 0) todo.Enqueue(t - a.y);
                    }
                }
            }
            string[] converted = visited.Select(x => x == 0 ? id : "0").ToArray();
            return converted;
        }

        public static void FillFloorFromArrayAndOffset(MapSector gl, GameLevel level, int x, int y, string[] arr, int sx, int sy) {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if(arr[i*y+j] != "0") {
                        level.SetFloor(sx + i + gl.SectorOffsetX * MapSector.Rx, sy + j + gl.SectorOffsetY * MapSector.Ry, arr[i * y + j]);
                    }
                }
            }
        }

        public static void ClearBlocksFromArrayAndOffset(MapSector gl, GameLevel level, int x, int y, string[] arr, int sx, int sy)
        {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (arr[i * y + j] != "0") {
                        level.SetBlock(sx + i + gl.SectorOffsetX * MapSector.Rx, sy + j + gl.SectorOffsetY * MapSector.Ry, "0");
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
