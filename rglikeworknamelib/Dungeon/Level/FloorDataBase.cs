using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class FloorDataBase {
        public static Dictionary<string, FloorData> Data;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public FloorDataBase() {
            Data = new Dictionary<string, FloorData>();
            var a = ParsersCore.UniversalParseDirectory(Settings.GetFloorDataDirectory(), UniversalParser.Parser<FloorData>, typeof(Floor));
            foreach (var pair in a)
            {
                Data.Add(pair.Key, (FloorData)pair.Value);
            }
        }
    }
}