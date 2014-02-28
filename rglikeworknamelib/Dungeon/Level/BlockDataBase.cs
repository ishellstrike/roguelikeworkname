using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class BlockAction
    {
        public string Name;
        public dynamic Action;

        public BlockAction(dynamic openCan, string s)
        {
            Name = s;
            Action = openCan;
        }
    }

    public class BlockDataBase {
        private static readonly Logger logger = LogManager.GetLogger("BlockDataBase");
        public static Dictionary<string, BlockData> Data;
        public static Dictionary<string, BlockData> Storages;
        public static Dictionary<string, BlockAction> BlockScripts; 


        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public BlockDataBase() {
            Storages = new Dictionary<string, BlockData>();

            Data = UniversalParser.JsonDictionaryDataLoader<BlockData>(Settings.GetObjectDataDirectory());

            foreach (var blockData in Data) {
                if (blockData.Value.Type != null) {
                    blockData.Value.TypeParsed =
                        Type.GetType(typeof (BlockDataBase).Namespace + "." + blockData.Value.Type);
                }
                else {
                    blockData.Value.TypeParsed = typeof (Block);
                }
            }

            foreach (var pair in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                Storages.Add(pair.Key, pair.Value);
            }

            var idb = ItemDataBase.Instance;

            int i;
            foreach (var source in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                foreach (var dropGroup in source.Value.ItemSpawn) {
                    for (i = 0; i < dropGroup.Ids.Count; i++) {
                        var id = dropGroup.Ids[i];
                        if (id.StartsWith("spawn_")) {
                            dropGroup.Ids.Remove(id);
                            dropGroup.Ids.AddRange(idb.GetBySpawnGroup(id).Select(itemData => itemData.Key));
                        }
                    }
                }
            }
        }

        public static void TrySpawnItems(Random rnd, Block block)
        {
            if (block.Data.ItemSpawn != null)
            {
                foreach (var drop in block.Data.ItemSpawn)
                {

                    for (int n = 0; n < drop.Repeat; n++)
                    {
                        var thr = rnd.Next(100) + 1;
                        if (drop.Prob >= thr)
                        {
                            var item = ItemFactory.GetInstance(drop.Ids[rnd.Next(drop.Ids.Count)],
                                rnd.Next(Math.Abs(drop.Max - drop.Min)) +
                                Math.Min(drop.Min, drop.Max));
                            if (item != null)
                            {
                                block.StoredItems.Add(item);
                            }
                        }
                    }
                }
            }
        }
    }
}