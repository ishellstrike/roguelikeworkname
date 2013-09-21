using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Item
{
    public class InventorySystem {
        private List<Item> items_ = new List<Item>();

        public int TotalWeight {
            get {
                var db = ItemDataBase.Data;
                return items_.Select(x => db[x.Id].Weight).Sum();
            }
        }

        public int TotalVolume
        {
            get
            {
                var db = ItemDataBase.Data;
                return items_.Select(x => db[x.Id].Volume).Sum();
            }
        }

        public void AddItem(Item it) {
            items_.Add(it);
            switch (ItemDataBase.Data[it.Id].SType) {
                    case ItemType.Ammo:
                    Achievements.Stat["ammototal"].Count += it.Count;
                    break;
                    case ItemType.Gun:
                    Achievements.Stat["guntotal"].Count += it.Count;
                    break;
                    case ItemType.Food:
                    Achievements.Stat["foodtotal"].Count += it.Count;
                    break;
            }
        }

        public void RemoveItem(Item it) {
            items_.Remove(it);
        }

        public bool ContainsItem(Item it) {
            return items_.Contains(it);
        }

        public List<Item> FilterByType(ItemType it) {
            return it == ItemType.Nothing ? items_ : items_.Where(item => ItemDataBase.Data[item.Id].SType == it).ToList();
        }

        public void StackSimilar() {
            var a = TakeDistinctItems();

            foreach (var item in items_) {
                FindById(a, item.Id).Count += item.Count;
            }

            items_ = a;
        }

        private static Item FindById(IEnumerable<Item> list, string id) {
            return list.FirstOrDefault(item => item.Id == id);
        }

        private List<Item> TakeDistinctItems() {
            var a = new List<Item>();

            foreach (var item in items_) {
                Item item1 = item;
                if(a.Select(x => x.Id == item1.Id).All(y => y == false)) a.Add(new Item(item));
            }

            foreach (var item in a) {
                item.Count = 0;
            }

            return a;
        }

        public void UseItem(Item selectedItem, Player player) {
            if (selectedItem == null || player == null) {
                return;
            }
            switch (ItemDataBase.Data[selectedItem.Id].SType)
            {
                case ItemType.Hat:
                case ItemType.Pants:
                case ItemType.Helmet:
                case ItemType.Meele:
                case ItemType.Shirt:
                case ItemType.Ammo:
                case ItemType.Bag:
                case ItemType.Boots:
                case ItemType.Glaces:
                case ItemType.Gloves:
                case ItemType.Gun:
                    player.EquipItem(selectedItem, this);
                    break;
                case ItemType.Food:
                    EventLog.Add(string.Format("Вы употребили {0}", ItemDataBase.Data[selectedItem.Id].Name), GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                    foreach (var buff in selectedItem.Buffs)
                    {
                        buff.ApplyToTarget(player);
                    }
                    if (items_.Contains(selectedItem))
                    {
                        items_.Remove(selectedItem);
                    }
                    break;
            }
        }

        public void AddItemRange(List<Item> inContainer) {
            foreach (var item in inContainer) {
                AddItem(item);
            }
        }
    }
}
