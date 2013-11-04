using System.Collections.Generic;
using System.IO;
using rglikeworknamelib.Dungeon.Level.Blocks;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockDataBase {
        public static Dictionary<string, BlockData> Data;
        public static Dictionary<string, BlockData> Storages;

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public BlockDataBase() {
            Data = new Dictionary<string, BlockData>();
            Storages = new Dictionary<string, BlockData>();
            List<KeyValuePair<string, object>> a =
                ParsersCore.UniversalParseDirectory(
                    Directory.GetCurrentDirectory() + @"/" + Settings.GetObjectDataDirectory(),
                    UniversalParser.Parser<BlockData>, typeof (Block));
            foreach (var pair in a) {
                Data.Add(pair.Key, (BlockData) pair.Value);

                if(((BlockData)pair.Value).Prototype == typeof(StorageBlock)) {
                    Storages.Add(pair.Key, (BlockData)pair.Value);
                }
            }
        }
    }
}