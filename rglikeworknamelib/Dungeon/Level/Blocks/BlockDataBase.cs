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

            Data = UniversalParser.JsonDataLoader<BlockData>(Settings.GetObjectDataDirectory());

            foreach (var pair in Data.Where(pair => pair.Value.SmartAction == SmartAction.ActionOpenContainer)) {
                Storages.Add(pair.Key, pair.Value);
            }

            foreach (var source in Data.Where(pair => pair.Value.ItemSpawn != null)) {
                foreach (var dropGroup in source.Value.ItemSpawn) {
                    for (int i = 0; i < dropGroup.Ids.Count; i++) {
                        var id = dropGroup.Ids[i];
                        if (id.StartsWith("spawn_")) {
                            dropGroup.Ids.Remove(id);
                            dropGroup.Ids.AddRange(ItemDataBase.GetBySpawnGroup(id).Select(itemData => itemData.Key));
                        }
                    }
                }
            }
        }
    }
}