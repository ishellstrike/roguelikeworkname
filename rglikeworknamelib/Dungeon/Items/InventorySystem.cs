﻿using System;
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
                return items_.Select(x => x.Data.Weight).Sum();
            }
        }

        public int TotalVolume
        {
            get
            {
                return items_.Select(x => x.Data.Volume).Sum();
            }
        }

        public void AddItem(Item it) {
            items_.Add(it);
            switch (it.Data.SType) {
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
            return it == ItemType.Nothing ? items_ : items_.Where(item => item.Data.SType == it).ToList();
        }

        public void StackSimilar() {
            var a = new List<Item>();

            foreach (var item in items_) {
                Item it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses != 0);
                if(it != null) {
                    it.Count += item.Count;
                }
                a.Add(item);
            }

            //foreach (var item in items_) {
            //    FindById(a, item.Id).Count += item.Count;
            //}

            items_ = a;
        }

        public void UseItem(Item selectedItem, Player player) {
            if (selectedItem == null || player == null) {
                return;
            }
            switch (selectedItem.Data.SType)
            {
                case ItemType.Wear:
                case ItemType.Meele:
                case ItemType.Ammo:
                case ItemType.Bag:
                case ItemType.Gun:
                    player.EquipItem(selectedItem, this);
                    break;
                case ItemType.Medicine:
                case ItemType.Food:
                    if(player.EatItem(selectedItem)) {
                        EventLog.Add(
                            string.Format("{1} {0}", selectedItem.Data.Name,
                                          selectedItem.Data.SType == ItemType.Medicine ? "Вы приняли" : "Вы употребили"),
                            GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                        foreach (var buff in selectedItem.Buffs) {
                            buff.ApplyToTarget(player);
                        }
                        selectedItem.Doses--;
                        if (selectedItem.Doses <= 0 && items_.Contains(selectedItem)) {
                            items_.Remove(selectedItem);
                        }
                    }
                    else {
                        EventLog.Add(
                            string.Format("Вы не можете съесть больше"),
                            GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                    }
                    break;
            }
        }

        public void AddItemRange(List<Item> inContainer) {
            foreach (var item in inContainer) {
                AddItem(item);
            }
        }

        public Item ContainsId(string itemData) {
            return items_.FirstOrDefault(x => x.Id == itemData);
        }
    }
}
