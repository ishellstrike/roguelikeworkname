using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items
{
    public class CraftData {
        public string[] Input1;
        public string[] Input1Count;

        public string[] Input2;
        public string[] Input2Count;

        public string[] Input3;
        public string[] Input3Count;

        public string[] Input4;
        public string[] Input4Count;

        public string[] Output;
        public string[] OutputCount;

        public CraftType SType = CraftType.Other;
    }

    public enum CraftType {
        Other,
        Misc,
        Food,
        Medicine,
        Weapon
    }

    public class CraftDataBase {
        public static Collection<CraftData> Data;

        public CraftDataBase() {
            Data = new Collection<CraftData>();
            var t = ParsersCore.UniversalParseDirectory(Settings.GetCraftsDirectory(), UniversalParser.NoIdParser<CraftData>);
            foreach (var value in t) {
                Data.Add((CraftData)value);
            }
        }
    }
}
