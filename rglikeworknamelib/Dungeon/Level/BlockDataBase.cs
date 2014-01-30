using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using rglikeworknamelib.Dungeon.Items;
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
            Storages = new Dictionary<string, BlockData>();

            Data = UniversalParser.JsonDictionaryDataLoader<BlockData>(Settings.GetObjectDataDirectory());

            foreach (var pair in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                Storages.Add(pair.Key, pair.Value);
            }

            var idb = ItemDataBase.Instance;

            foreach (var source in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                foreach (var dropGroup in source.Value.ItemSpawn) {
                    for (int i = 0; i < dropGroup.Ids.Count; i++) {
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
                                rnd.Next(drop.Max - drop.Min) +
                                drop.Min);
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