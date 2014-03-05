using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;
using rglikeworknamelib.Dialogs;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Generation.Names;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib
{
    public class Registry:Singleton<Registry> {
        private static readonly Logger logger = LogManager.GetLogger("JargRegistry");
        public Dictionary<string, BlockData> Blocks;
        public Dictionary<string, FloorData> Floors;
        public Dictionary<string, ItemData> Items;
        public List<ItemCraftData> Craft;
        public List<Schemes> Schemes;
        public List<ItemCraftData> Crafts;
        public List<ItemModiferData> ItemModifers;
        public Dictionary<string, CreatureData> Creatures;
        
        protected Registry() {
            BlockBdInit();
            ItemBdInit();
            Floors = new Dictionary<string, FloorData>();
            List<KeyValuePair<string, object>> a = ParsersCore.UniversalParseDirectory(
                Settings.GetFloorDataDirectory(), UniversalParser.Parser<FloorData>, typeof(Floor));
            foreach (var pair in a)
            {
                Floors.Add(pair.Key, (FloorData)pair.Value);
            }
            Creatures = UniversalParser.JsonDictionaryDataLoader<CreatureData>(Settings.GetCreatureDataDirectory());
        }
        public void Init() {
            foreach (var source in Blocks.Where(pair => pair.Value.ItemSpawn != null))
            {
                foreach (var dropGroup in source.Value.ItemSpawn) {
                    for (int i = 0; i < dropGroup.Ids.Count; i++)
                    {
                        var id = dropGroup.Ids[i];
                        if (id.StartsWith("spawn_"))
                        {
                            dropGroup.Ids.Remove(id);
                            dropGroup.Ids.AddRange(GetBySpawnGroup(id).Select(itemData => itemData.Key));
                        }
                    }
                }
            }
        }
        private void BlockBdInit() {
            Blocks = UniversalParser.JsonDictionaryDataLoader<BlockData>(Settings.GetObjectDataDirectory());

            foreach (var blockData in Blocks)
            {
                if (blockData.Value.Type != null)
                {
                    blockData.Value.TypeParsed =
                        Type.GetType(typeof(Block).Namespace + "." + blockData.Value.Type);
                }
                else
                {
                    blockData.Value.TypeParsed = typeof(Block);
                }
            }
        }
        private void ItemBdInit() {
            Items = UniversalParser.JsonDictionaryDataLoader<ItemData>(Settings.GetItemDataDirectory());
            foreach (var itemData in Items)
            {
                if (itemData.Value.Type != null)
                {
                    itemData.Value.TypeParsed = Type.GetType(typeof(Item).Namespace + "." + itemData.Value.Type);
                }
                else
                {
                    itemData.Value.TypeParsed = typeof(Item);
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
        public string GetItemDescription(Item i)
        {
            return i.Data.Description;
        }
        public static List<KeyValuePair<string, ItemData>> GetBySpawnGroup(string group)
        {
            return Instance.Items.Where(x => x.Value.SpawnGroup == group).ToList();
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
            ItemData item = Items[i.Id];
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
                sb.Append(Environment.NewLine + string.Format("оставляет {0}", Items[item.AfteruseId].Name));
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

                sb.Append(Environment.NewLine + Items[item.Ammo].Name);
            }
            //switch (item.SortType) {
            //    case ItemType.Medicine:
            //        sb.Append(item.)
            //        break;
            //}
            return sb.ToString();
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
                            if (item != null && block is IItemStorage)
                            {
                                ((IItemStorage)block).AddItem(item);
                            }
                        }
                    }
                }
            }
        }

        public string LoadSummaryString(Stopwatch sw) {
            return string.Format(
                "\nTotal:\n     {1} Monsters\n     {2} Blocks\n     {3} Floors\n     {4} Items\n     {5} Schemes\n     {6} Buffs\n     {7} Dialogs\n     {8} Names\n     {9} Crafts\n     loaded in {0}",
                sw.Elapsed,
                Creatures.Count,
                Blocks.Count,
                Floors.Count,
                Items.Count,
                SchemesDataBase.Data.Count,
                BuffDataBase.Data.Count,
                DialogDataBase.data.Count,
                NameDataBase.data.Count,
                Craft.Count
                );
        }
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
