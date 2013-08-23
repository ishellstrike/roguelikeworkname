using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDataBase {
        public static Dictionary<string, BlockData> Data;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public BlockDataBase() {
            Data = new Dictionary<string, BlockData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<string, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetObjectDataDirectory(), BlockParser.Parser);
            foreach (var pair in a) {
                Data.Add(pair.Key, (BlockData)pair.Value);
            }
        }
    }
}