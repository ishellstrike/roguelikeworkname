using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class FloorDataBase {
        public Dictionary<int, FloorData> Data;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public FloorDataBase() {
            Data = new Dictionary<int, FloorData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<int, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetFloorDataDirectory(), FloorParser.Parser);
            foreach (var pair in a)
            {
                Data.Add(pair.Key, (FloorData)pair.Value);
            }
        }

        public FloorDataBase(Dictionary<int, FloorData> t)
        {
            Data = t;
        }
    }
}