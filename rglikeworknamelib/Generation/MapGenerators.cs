using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Generation
{
    internal static class MapGenerators
    {
        static Random rnd = new Random();
        public static void GenerateTest1(ref Block[] bb, ref Floor[] ff, int rx, int ry)
        {
            foreach (var floor in ff) {
                floor.ID = 1;
            }
            GenerateRoads1(ref bb, ref ff, rx, ry, 100, 10, 2);
        }

        public static void FillTest1(ref Block[] bb, ref Floor[] ff, int rx, int ry, int id)
        {
            foreach (var floor in ff) {
                floor.ID = id;
            }
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

        public static void GenerateStreets(ref List<Street> st, int rx, int ry, int count, int len, int flid, int trid)
        {
            float widthdiv2 = 3;
            int trotwid = 2;

            int tx = rnd.Next(0, rx);
            int ty = rnd.Next(0, ry);

            for (int i = 0; i < count; i++) {
                int dx = rnd.Next(-1, 1);
                int dy = 0;
                if (dx == 0) dy = rnd.Next(0, 1) == 0 ? 1 : -1;
                Street tmp = new Street();
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

        public static void FillStreets(List<Street> st, ref Floor[] ff, int rx, int ry)
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

        internal static void AddTestScheme(GameLevel gl, SchemesDataBase sch, int rx, int ry)
        {
            Vector2 start = new Vector2(0, 0);
            var scheme = sch.Data[0];

            for (int i = 0; i < scheme.x; i++) {
                for (int j = 0; j < scheme.y; j++) {
                    gl.CreateBlock((int)start.X + i, (int)start.Y + j, scheme.data[i * scheme.y + j]);
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
    }
}
