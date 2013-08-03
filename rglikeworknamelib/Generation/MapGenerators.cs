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
        public static void FillTest1(MapSector gl, int id)
        {
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    gl.SetFloor(i, j, id);
                }
            }
        }
        //blanks 4 5 6 7
        public static void GenerateStreetsNew(MapSector mapSector, int streetCount, int streetLength, int step, int floorId, int trotuarId, Random rnd)
        {
            List<MinMax> streets = new List<MinMax>();
            List<Point> sNodes = new List<Point>();
            List<Point> visitedSNodes = new List<Point>();

            sNodes.Add(new Point(GetRandomCoordInCenter(MapSector.Rx, rnd), GetRandomCoordInCenter(MapSector.Ry,rnd)));

            for (int i = 0; i < streetCount; i++) {
                int num = rnd.Next(0, sNodes.Count);
                Point cur = sNodes[num];
                visitedSNodes.Add(cur);
                sNodes.RemoveAt(num);

                int curlen = rnd.Next(step/2, streetLength);
                int curoffs = rnd.Next(0, curlen/step + 1) * step;

                bool isHorizontal = rnd.Next(0, 2) == 0;

                if (isHorizontal) {
                    streets.Add(new MinMax(cur.X - curoffs, cur.Y, cur.X + curlen - curoffs, cur.Y));
                } else {
                   streets.Add(new MinMax(cur.X, cur.Y - curoffs, cur.X, cur.Y + curlen - curoffs));
                }

                for (int j = 0; j < curlen; j+= step) {
                    if (isHorizontal) {
                        sNodes.Add(new Point(cur.X - curoffs + j, cur.Y));
                    } else {
                        sNodes.Add(new Point(cur.X, cur.Y - curoffs + j));
                    }
                }
            }

            FillStreetsNew(streets, mapSector, floorId, trotuarId);

            visitedSNodes = visitedSNodes.Distinct().ToList();


            List<MinMax> squares = GetSquaresFromNodes(visitedSNodes);

            FillMinMaxRandomly(squares, mapSector, new int[] {3}, rnd);
    }

        public static List<MinMax> GetSquaresFromNodes(List<Point> visitedSNodes)
        {
            List<MinMax> squares = new List<MinMax>();
            foreach (var a in visitedSNodes) {
                List<int> tempx = visitedSNodes.Where(c => (c.X > a.X) && (c.Y == a.Y)).Select(c => c.X).ToList();
                List<int> tempy = visitedSNodes.Where(c => (c.Y > a.Y) && (c.X == a.X)).Select(c => c.Y).ToList();


                float minx;
                if (tempx.Count != 0) {
                    minx = tempx.Min();
                } else continue;

                float miny;
                if (tempy.Count != 0) {
                    miny = tempy.Min();
                } else continue;

                squares.Add(new MinMax(a.X, a.Y, minx, miny));
            }

            return squares;
        }

        public static void FillStreetsNew(List<MinMax> st, MapSector gl, int flid, int trid)
        {
            foreach (var street in st) {
                for (int i = (int)street.Min.X - 1; i < (int)street.Max.X + 4; i++) {
                    for (int j = (int)street.Min.Y - 1; j < (int)street.Max.Y + 4; j++) {
                        if (i < MapSector.Rx && j < MapSector.Ry && i >= 0 && j >= 0) gl.SetFloor(i, j, trid);
                    }
                }
            }

            foreach (var street in st) {
                for (int i = (int)street.Min.X; i < (int)street.Max.X + 3; i++) {
                    for (int j = (int)street.Min.Y; j < (int)street.Max.Y + 3; j++) {
                        if (i < MapSector.Rx && j < MapSector.Ry && i >= 0 && j >= 0) gl.SetFloor(i, j, flid);
                    }
                }
            }
        }

        public static void FillMinMaxRandomly(List<MinMax> st, MapSector gl, int[] arrid, Random rnd)
        {
            foreach (var street in st) {
                int tempcol = arrid[rnd.Next(0, arrid.Length)];
                for (int i = (int)street.Min.X + 4; i < (int)street.Max.X - 1; i++) {
                    for (int j = (int)street.Min.Y + 4; j < (int)street.Max.Y - 1; j++) {
                        if (i < MapSector.Rx && j < MapSector.Ry && i >= 0 && j >= 0) gl.SetFloor(i, j, tempcol);
                    }
                }
            }
        }

        private static int GetRandomCoordInCenter(int rx, Random rnd)
        {
            return rnd.Next(-rx/10,rx/10)+rx/2;
        }

        public static void GenerateRoads1(ref Block[] bb, ref Floor[] ff, int rx, int ry, int count, int len, int flid, Random rnd)
        {
            int tx = rnd.Next(0, rx);
            int ty = rnd.Next(0, ry);
            for (int i = 0; i < count; i++) {
                int dx = rnd.Next(-1, 1);
                int dy = 0;
                if (dx == 0) dy = rnd.Next(0, 1) == 0 ? 1 : -1;
                for (int j = 0; j < len; j++) {
                    tx += dx;
                    ty += dy;
                    if (tx >= rx) tx = 0;
                    if (ty >= ry) ty = 0;
                    if (tx < 0) tx = rx - 1;
                    if (ty < 0) ty = ry - 1;
                    ff[tx * ry + ty].Id = flid;
                }
            }
        }

        public static void GenerateStreets(ref List<StreetOld__> st, int rx, int ry, int count, int len, int flid, int trid, Random rnd)
        {
            float widthdiv2 = 3;
            int trotwid = 2;

            int tx = rnd.Next(0, rx);
            int ty = rnd.Next(0, ry);

            for (int i = 0; i < count; i++) {
                int dx = rnd.Next(-1, 1);
                int dy = 0;
                if (dx == 0) dy = rnd.Next(0, 1) == 0 ? 1 : -1;
                StreetOld__ tmp = new StreetOld__();
                tmp.widthdiv2 = widthdiv2;
                tmp.trotwidth = trotwid;
                tmp.id = flid;
                tmp.tid = trid;
                tmp.from = new Vector2(tx - widthdiv2 + 1, ty - widthdiv2 + 1);
                tx += dx * len;
                ty += dy * len;
                if (tx >= rx) tx = 0;
                if (ty >= ry) ty = 0;
                if (tx < 0) tx = rx - 1;
                if (ty < 0) ty = ry - 1;
                tmp.to = new Vector2(tx + widthdiv2 - 1, ty + widthdiv2 - 1);
                st.Add(tmp);
            }
        }

        public static void FillStreets(List<StreetOld__> st, ref Floor[] ff, int rx, int ry, Random rnd)
        {
            foreach (var street in st) {
                for (int i = (int)street.from.X - (int)street.trotwidth; i < (int)street.to.X + 1 + street.trotwidth; i++) {
                    for (int j = (int)street.from.Y - (int)street.trotwidth; j < (int)street.to.Y + 1 + street.trotwidth; j++) {
                        if (i < rx && j < ry && i >= 0 && j >= 0) ff[i * rx + j].Id = street.tid;
                    }
                }
            }

            foreach (var street in st) {
                for (int i = (int)street.from.X; i < (int)street.to.X + 1; i++) {
                    for (int j = (int)street.from.Y; j < (int)street.to.Y + 1; j++) {
                        if (i < rx && j < ry && i >= 0 && j >= 0) ff[i * rx + j].Id = street.id;
                    }
                }
            }
        }

        internal static void PlaceScheme(MapSector gl, Schemes scheme, int x, int y)
        {
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    if (x + i < MapSector.Rx && y + j < MapSector.Ry) {
                        if (scheme.data[i * scheme.y + j] != 0) {
                            gl.SetBlock(x + i, y + j, scheme.data[i*scheme.y + j]);
                        }
                    }
                }
            }
        }

        internal static void PlaceRandomSchemeByType(MapSector mapSector, SchemesType schemeType, int posX, int posY, Random rnd)
        {
            List<Schemes> a;
            switch (schemeType) {
                case SchemesType.house:
                    a = mapSector.SchemesDataBase.Houses;
                    break;

                default:
                    a = mapSector.SchemesDataBase.Data.Where(x => x.type == schemeType).ToList();
                    break;;
            }

            if(a.Count > 0) {
                int r = rnd.Next(0, a.Count);
                PlaceScheme(mapSector, a[r], posX, posY);
                FillFloorFromArrayAndOffset(mapSector, a[r].x, a[r].y, GetInnerFloorArrayWithId(a[r], 8), posX, posY);
            }
        }

        internal static void AddTestScheme(GameLevel gl, SchemesDataBase sch, int rx, int ry, Random rnd)
        {
            Vector2 start = new Vector2(0, 0);
            Schemes scheme = sch.Data[0];

            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    gl.SetBlock((int)start.X + i, (int)start.Y + j, scheme.data[i * scheme.y + j]);
                }
            }

            List<StorageBlock> storageBlocks = gl.GetStorageBlocks();

            foreach (var storageBlock in storageBlocks) {
                for (int i = 0; i < 10; i++) {
                    storageBlock.StoredItems.Add(new Item {
                        Count = 3,
                        Id = rnd.Next(1,19)
                    });
                }
                
            }
        }

        public static void ClearBlocks(MapSector gameLevel) {
            for (int i = 0; i < MapSector.Rx * MapSector.Ry; i++) {
                gameLevel.SetBlock(i, 0);
            }
        }

        public static int[] GetInnerFloorArrayWithId(Schemes a, int id) {
            int[] visited = new int[a.data.Length];
            Queue<int> todo = new Queue<int>();
            if (a.data[0] == 0) todo.Enqueue(0); // угол 0,0
            if (a.data[(a.x - 1) * a.y] == 0) todo.Enqueue((a.x - 1) * a.y); //угол х,0
            if (a.data[a.y - 1] == 0) todo.Enqueue(a.y - 1); //угол 0,у
            if (a.data[(a.x - 1) * a.y + a.y - 1] == 0) todo.Enqueue((a.x - 1) *a.y + a.y - 1); //угол х,у


            while (todo.Count > 0) {
                int t = todo.Dequeue();

                if(visited[t] == 0 && a.data[t] == 0) {
                    visited[t] = 1;
                    if (t % a.y < a.x - 1) todo.Enqueue(t + 1);
                    if (t % a.y > 0) todo.Enqueue(t - 1);
                    if (t / a.y < a.y - 1) todo.Enqueue(t + a.y);
                    if (t / a.y > 0) todo.Enqueue(t - a.y);
                }
            }

            visited = visited.Select(x => x == 0 ? id : 0).ToArray();

            return visited;
        }

        public static void FillFloorFromArrayAndOffset(MapSector gl, int x, int y, int[] arr, int sx, int sy) {
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    if (sx + i < MapSector.Rx && sy + j < MapSector.Ry) {
                        if(arr[i*y+j] != 0) {
                            gl.SetFloor(sx + i, sy + j, arr[i * y + j]);
                        }
                    }
                }
            }
        }

        private static double Noise(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;
            double value = (1.0f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
            value = Math.Abs(value);
            return value;
        }

        private static double[] NoiseMap(int x, int y, int sx, int sy)
        {
            double[] a = new double[sx * sy];

            for (int i = 0; i < sx; i++) {
                for (int j = 0; j < sy; j++) {
                    a[i * sy + j] = Noise(i + x * sx, j + y * sy);
                }
            }
            return a;
        }

        private static double[] SmoothNoiseMap(int x, int y, int sx, int sy) {
            double[] a = NoiseMap(x, y, sx, sy);
            for (int i = 0; i < sx - 1; i++) {
                for (int j = 0; j < sy - 1; j++) {
                    double round = (byte) ((a[(i)*sy + j] + a[(i + 1)*sy + j] + a[(i + 1)*sy + j + 1] + a[(i)*sy + j + 1])/4);
                    a[(i)*sy + j] = ((a[(i)*sy + j] + round)/2);
                    a[(i + 1)*sy + j] = ((a[(i + 1)*sy + j] + round)/2);
                    a[(i)*sy + j + 1] = ((a[(i)*sy + j + 1] + round)/2);
                    a[(i + 1)*sy + j + 1] = ((a[(i + 1)*sy + j + 1] + round)/2);
                }
            }
            return a;
        }

        internal static void FloorPerlin(MapSector mapSector) {
            double[] a = SmoothNoiseMap(mapSector.SectorOffsetX, mapSector.SectorOffsetY, MapSector.Rx, MapSector.Ry);
            for (int i = 0; i < MapSector.Rx; i++) {
                for (int j = 0; j < MapSector.Ry; j++) {
                    double t = a[i * MapSector.Ry + j];
                    if (t > 0.04) {
                        mapSector.SetFloor(i, j, 1);
                    } else {
                        if (t > 0.08) {
                            mapSector.SetFloor(i, j, 2);
                        } else {
                            mapSector.SetFloor(i, j, 3);
                        }
                    }
                }
            }
        }
    }
}
