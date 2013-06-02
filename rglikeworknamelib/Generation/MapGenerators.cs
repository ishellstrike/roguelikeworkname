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
        static Random rnd = new Random();

        public static void FillTest1(GameLevel gl, int id)
        {
            for (int i = 0; i < gl.rx; i++) {
                for (int j = 0; j < gl.ry; j++) {
                    gl.SetFloor(i, j, id);
                }
            }
        }
        //blanks 4 5 6 7
        public static void GenerateStreetsNew(GameLevel gl, int count, int len, int step, int flid, int trid) {
            var streets = new List<MinMax>();
            var sNodes = new List<Point>();
            var visitedSNodes = new List<Point>();

            sNodes.Add(new Point(GetRandomCoordInCenter(gl.rx), GetRandomCoordInCenter(gl.ry)));

            for (int i = 0; i < count; i++) {
                int num = rnd.Next(0, sNodes.Count);
                var cur = sNodes[num];
                visitedSNodes.Add(cur);
                sNodes.RemoveAt(num);

                int curlen = rnd.Next(step/2, len);
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

            FillStreetsNew(streets, gl, flid, trid);

            visitedSNodes = visitedSNodes.Distinct().ToList();


            var squares = GetSquaresFromNodes(visitedSNodes);

            FillMinMaxRandomly(squares, gl, new int[] {3});
    }

        public static List<MinMax> GetSquaresFromNodes(List<Point> visitedSNodes)
        {
            List<MinMax> squares = new List<MinMax>();
            foreach (var a in visitedSNodes) {
                var tempx = visitedSNodes.Where(c => (c.X > a.X) && (c.Y == a.Y)).Select(c => c.X).ToList();
                var tempy = visitedSNodes.Where(c => (c.Y > a.Y) && (c.X == a.X)).Select(c => c.Y).ToList();


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

        public static void FillStreetsNew(List<MinMax> st, GameLevel gl, int flid, int trid)
        {
            foreach (var street in st) {
                for (int i = (int)street.Min.X - 1; i < (int)street.Max.X + 4; i++) {
                    for (int j = (int)street.Min.Y - 1; j < (int)street.Max.Y + 4; j++) {
                        if (i < gl.rx && j < gl.ry && i >= 0 && j >= 0) gl.SetFloor(i, j, trid);
                    }
                }
            }

            foreach (var street in st) {
                for (int i = (int)street.Min.X; i < (int)street.Max.X + 3; i++) {
                    for (int j = (int)street.Min.Y; j < (int)street.Max.Y + 3; j++) {
                        if (i < gl.rx && j < gl.ry && i >= 0 && j >= 0) gl.SetFloor(i, j, flid);
                    }
                }
            }
        }

        public static void FillMinMaxRandomly(List<MinMax> st, GameLevel gl, int[] arrid)
        {
            foreach (var street in st) {
                var tempcol = arrid[rnd.Next(0, arrid.Length)];
                for (int i = (int)street.Min.X + 4; i < (int)street.Max.X - 1; i++) {
                    for (int j = (int)street.Min.Y + 4; j < (int)street.Max.Y - 1; j++) {
                        if (i < gl.rx && j < gl.ry && i >= 0 && j >= 0) gl.SetFloor(i, j, tempcol);
                    }
                }
            }
        }

        private static int GetRandomCoordInCenter(int rx) {
            return rnd.Next(-rx/10,rx/10)+rx/2;
        }

        public static void GenerateRoads1(ref Block[] bb, ref Floor[] ff, int rx, int ry, int count, int len, int flid)
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
                    ff[tx * ry + ty].ID = flid;
                }
            }
        }

        public static void GenerateStreets(ref List<StreetOld__> st, int rx, int ry, int count, int len, int flid, int trid)
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

        public static void FillStreets(List<StreetOld__> st, ref Floor[] ff, int rx, int ry)
        {
            foreach (var street in st) {
                for (int i = (int)street.from.X - (int)street.trotwidth; i < (int)street.to.X + 1 + street.trotwidth; i++) {
                    for (int j = (int)street.from.Y - (int)street.trotwidth; j < (int)street.to.Y + 1 + street.trotwidth; j++) {
                        if (i < rx && j < ry && i >= 0 && j >= 0) ff[i * rx + j].ID = street.tid;
                    }
                }
            }

            foreach (var street in st) {
                for (int i = (int)street.from.X; i < (int)street.to.X + 1; i++) {
                    for (int j = (int)street.from.Y; j < (int)street.to.Y + 1; j++) {
                        if (i < rx && j < ry && i >= 0 && j >= 0) ff[i * rx + j].ID = street.id;
                    }
                }
            }
        }

        internal static void PlaceScheme(GameLevel gl, Schemes scheme, int x, int y)
        {
            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    if (x + i < gl.rx && y + j < gl.ry) {
                        if (scheme.data[i * scheme.y + j] != 0) {
                            gl.SetBlock(x + i, y + j, scheme.data[i*scheme.y + j]);
                        }
                    }
                }
            }
        }

        internal static void PlaceRandomSchemeByType(GameLevel gl, SchemesType st, int rx, int ry) {
            List<Schemes> a;
            switch (st) {
                case SchemesType.house:
                    a = gl.SchemesBase.Houses;
                    break;

                default:
                    a = gl.SchemesBase.Data.Where(x => x.type == st).ToList();
                    break;;
            }
           

            if(a.Count > 0) {
                int r = rnd.Next(0, a.Count);
                PlaceScheme(gl, a[r], rx, ry);
             
            }
        }

        internal static void AddTestScheme(GameLevel gl, SchemesDataBase sch, int rx, int ry)
        {
            Vector2 start = new Vector2(0, 0);
            var scheme = sch.Data[0];

            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    gl.SetBlock((int)start.X + i, (int)start.Y + j, scheme.data[i * scheme.y + j]);
                }
            }

            List<StorageBlock> storageBlocks = gl.GetStorageBlocks();

            foreach (var storageBlock in storageBlocks) {
                for (int i = 0; i < 10; i++) {
                    storageBlock.storedItems.Add(new Item {
                        count = 3,
                        id = rnd.Next(1,19)
                    });
                }
                
            }
        }

        public static void ClearBlocks(GameLevel gameLevel) {
            for(int i=0;i<gameLevel.rx*gameLevel.ry;i++) {
                gameLevel.SetBlock(i, 0);
            }
        }
    }
}
