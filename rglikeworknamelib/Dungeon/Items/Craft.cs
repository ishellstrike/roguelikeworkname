using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items
{
    public class CraftData {
        public Item[] Input1;
        public Item[] Input1Count;

        public Item[] Input2;
        public Item[] Input2Count;

        public Item[] Input3;
        public Item[] Input3Count;

        public Item[] Input4;
        public Item[] Input4Count;

        public Item[] Output;
        public Item[] OutputCount;
    }

    public class CraftDataBase {
        public static Dictionary<string, CraftData> Data;

        public CraftDataBase() {
            Data = new Dictionary<string, CraftData>();
            var t = ParsersCore.UniversalParseDirectory(Settings.GetCraftsDirectory(), UniversalParser.Parser<CraftData>);
            foreach (var pair in t) {
                Data.Add(pair.Key, (CraftData)pair.Value);
            }
        }
    }
}
