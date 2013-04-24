using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDataBase {
        public Dictionary<int, BlockData> Data;

        public BlockDataBase() {
            Data = new Dictionary<int, BlockData>();
            var a = ParsersCore.ParseDirectory<KeyValuePair<int, object>>(Directory.GetCurrentDirectory() + @"/" + Settings.GetObjectDataDirectory(), BlockParser.Parser);
            foreach (var pair in a) {
                Data.Add(pair.Key, (BlockData)pair.Value);
            }
        }

        public BlockDataBase(Dictionary<int, BlockData> t)
        {
            Data = t;
        }
    }
}