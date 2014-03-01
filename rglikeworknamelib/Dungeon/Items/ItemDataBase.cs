using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jarg;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dungeon.Items
{
    public sealed class ItemDataBase
    {
        private static readonly Logger logger = LogManager.GetLogger("ItemDataBase");

        public Dictionary<string, ItemData> Data;

        public List<ItemCraftData> Craft;
        private static ItemDataBase instance_;
        public List<ItemModiferData> ItemModifers; 

        /// <summary>
        ///     WARNING! Also loading all data from standart patch
        /// </summary>
        private ItemDataBase()
        {
            //texatlas = texatlas_;
            Data = UniversalParser.JsonDictionaryDataLoader<ItemData>(Settings.GetItemDataDirectory());
            foreach (var itemData in Data) {
                if (itemData.Value.Type != null) {
                    itemData.Value.TypeParsed = Type.GetType(typeof (ItemDataBase).Namespace + "." + itemData.Value.Type);
                }
                else {
                    itemData.Value.TypeParsed = typeof (Item);
                }
            }
            Craft = UniversalParser.JsonListDataLoader<ItemCraftData>(Settings.GetCraftsDirectory());

            ItemModifers = new List<ItemModiferData>();
            ItemModifers.Add(new ItemModiferData { Name = string.Empty });
            ItemModifers.Add(new ItemModiferData { Name = "Разогретый" });
            ItemModifers.Add(new ItemModiferData { Name = "Приготовленный" });
            ItemModifers.Add(new ItemModiferData { Name = "Пережареный" });
            ItemModifers.Add(new ItemModiferData { Name = "Обуглившийся" });
        }

        /// <summary>
        ///     Первое обращение порождает первичную загрузку
        /// </summary>
        public static ItemDataBase Instance
        {
            get { return instance_ ?? (instance_ = new ItemDataBase()); }
        }

        public string GetItemDescription(Item i)
        {
            return i.Data.Description;
        }

        public List<KeyValuePair<string, ItemData>> GetBySpawnGroup(string group)
        {
            return Data.Where(x => x.Value.SpawnGroup == group).ToList();
        }

        public static void StackSimilar(ref List<Item> items)
        {
            var a = new List<Item>();

            foreach (Item item in items)
            {
                Item it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses == 0);
                if (it != null)
                {
                    it.Count += item.Count;
                }
                else
                {
                    a.Add(item);
                }
            }

            items = a;
        }

        public string GetItemFullDescription(Item i)
        {
            ItemData item = Data[i.Id];
            var sb = new StringBuilder();
            sb.Append(item.Name);
            if (item.Description != null)
            {
                sb.Append(Environment.NewLine + item.Description);
            }
            sb.Append(Environment.NewLine + string.Format("{0} г", item.Weight));
            sb.Append(Environment.NewLine + string.Format("{0} места", item.Volume));
            if (Settings.DebugInfo)
            {
                sb.Append(Environment.NewLine + string.Format("id {0} uid {1}", i.Id, 0));
            }

            if (item.AfteruseId != null)
            {
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Data[item.AfteruseId].Name));
            }

            if (item.Buff != null)
            {
                sb.Append(Environment.NewLine + Environment.NewLine + string.Format("Эффекты :"));

                foreach (string buff in item.Buff)
                {
                    sb.Append(Environment.NewLine + string.Format("{0}", BuffDataBase.Data[buff].Name));
                }
            }
            if (item.Ammo != null)
            {
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

        #region ItemScripts

        public void OpenBottle(Player p, Item target)
        {
            p.Inventory.TryRemoveItem(target.Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(Data[target.Id].AfteruseId, 1));
        }

        #endregion
    }

    public enum ItemModifer
    {
        Nothing,
        Razogretyi,
        Prigotovlenniy,
        Perejareniy,
        Obuglivshiysa
    }

    public class ItemModiferData
    {
        public string Name;
    }
}