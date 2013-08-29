using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDataBase {
        public static Dictionary<string, BlockData> Data;

        /// <summary>
        /// WARNING! Also loading all data from standart patch
        /// </summary>
        public BlockDataBase() {
            Data = new Dictionary<string, BlockData>();
            var a = ParsersCore.UniversalParseDirectory<KeyValuePair<string, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetObjectDataDirectory(), UniversalParser.Parser<BlockData>, typeof(Block));
            foreach (var pair in a) {
                Data.Add(pair.Key, (BlockData)pair.Value);
            }
        }
    }
}