using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemDataBase {
        public static Dictionary<string, ItemData> Data;
        public static Dictionary<string, ItemData> DataMedicineItems;
        public static Dictionary<string, ItemData> DataFoodItems;

        public static Dictionary<string, List<DropGroup>> SpawnLists; 

        private static Logger logger_ = LogManager.GetCurrentClassLogger();
        // public Collection<Texture2D> texatlas;

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        public ItemDataBase() {
            //texatlas = texatlas_;
            Data = new Dictionary<string, ItemData>();
            DataFoodItems = new Dictionary<string, ItemData>();
            DataMedicineItems = new Dictionary<string, ItemData>();
            List<KeyValuePair<string, object>> a = ParsersCore.UniversalParseDirectory(Settings.GetItemDataDirectory(),
                                                                                       UniversalParser.Parser<ItemData>, typeof(Subclases.Item));
            foreach (var pair in a) {
                Data.Add(pair.Key, (ItemData) pair.Value);
                if (((ItemData) pair.Value).SortType == ItemType.Medicine) {
                    DataMedicineItems.Add(pair.Key, (ItemData) pair.Value);
                }
                if (((ItemData) pair.Value).SortType == ItemType.Food) {
                    DataFoodItems.Add(pair.Key, (ItemData) pair.Value);
                }
            }

            SpawnLists = new Dictionary<string, List<DropGroup>>();

            var b = ParsersCore.ParseDirectory(Settings.GetSpawnlistsDataDirectory(), SpawnlistParser.Parser);

            foreach (var pair in b) {
                if(SpawnLists.ContainsKey(pair.Key)) {
                    SpawnLists[pair.Key].Add(pair.Value);
                }
                else {
                    SpawnLists.Add(pair.Key, new List<DropGroup>{ pair.Value });
                }
            }
        }

        public static Dictionary<string, ItemData> GetItemByItemDataSortType(ItemType it) {
            switch (it) {
                case ItemType.Nothing:
                    return Data;
                case ItemType.Medicine:
                    return DataMedicineItems;
                case ItemType.Food:
                    return DataFoodItems;
                default:
                    return null;
            }
        }

        public static string GetItemDescription(IItem i) {
            return i.Data.Description;
        }

        public static List<KeyValuePair<string,ItemData>> GetBySpawnGroup(string group) {
            return Data.Where(x => x.Value.SpawnGroup == group).ToList();
        }

        public static string GetItemFullDescription(IItem i) {
            ItemData item = Data[i.Id];
            var sb = new StringBuilder();
            sb.Append(item.Name);
            if (item.Description != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + item.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} г", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} места", item.Volume));
            if (Settings.DebugInfo) {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, 0));
            }

            if (item.AfteruseId != null) {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Data[item.AfteruseId].Name));
            }

            if (item.Buff != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Эффекты :"));

                foreach (string buff in item.Buff) {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null) {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Калибр :"));

                sb.Append(Environment.NewLine + Data[item.Ammo].Name);
            }
            //switch (item.SortType) {
            //    case ItemType.Medicine:
            //        sb.Append(item.)
            //        break;
            //}
            return sb.ToString();
        }
    }
}