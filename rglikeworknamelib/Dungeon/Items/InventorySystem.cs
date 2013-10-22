using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Item {
    public class InventorySystem {
        private List<Items.Item> items_ = new List<Items.Item>();

        public int TotalWeight {
            get { return items_.Select(x => x.Data.Weight).Sum(); }
        }

        public int TotalVolume {
            get { return items_.Select(x => x.Data.Volume).Sum(); }
        }

        public void AddItem(Items.Item it) {
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

        public void RemoveItem(Items.Item it) {
            items_.Remove(it);
        }

        public bool ContainsItem(Items.Item it) {
            return items_.Contains(it);
        }

        public List<Items.Item> FilterByType(ItemType it) {
            return it == ItemType.Nothing ? items_ : items_.Where(item => item.Data.SType == it).ToList();
        }

        public void StackSimilar() {
            var a = new List<Items.Item>();

            foreach (Items.Item item in items_) {
                Items.Item it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses != 0);
                if (it != null) {
                    it.Count += item.Count;
                }
                a.Add(item);
            }

            //foreach (var item in items_) {
            //    FindById(a, item.Id).Count += item.Count;
            //}

            items_ = a;
        }

        public void UseItem(Items.Item selectedItem, Player player) {
            if (selectedItem == null || player == null) {
                return;
            }
            switch (selectedItem.Data.SType) {
                case ItemType.Wear:
                case ItemType.Meele:
                case ItemType.Ammo:
                case ItemType.Bag:
                case ItemType.Gun:
                    player.EquipItem(selectedItem, this);
                    break;
                case ItemType.Medicine:
                case ItemType.Food:
                    if (player.EatItem(selectedItem)) {
                        EventLog.Add(
                            string.Format("{1} {0}", selectedItem.Data.Name,
                                          selectedItem.Data.SType == ItemType.Medicine ? "Вы приняли" : "Вы употребили"),
                            GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                        foreach (IBuff buff in selectedItem.Buffs) {
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

        public void AddItemRange(List<Items.Item> inContainer) {
            foreach (Items.Item item in inContainer) {
                AddItem(item);
            }
        }

        public Items.Item ContainsId(string itemData) {
            return items_.FirstOrDefault(x => x.Id == itemData);
        }

        public void Save() {
            var binaryFormatter = new BinaryFormatter();
            var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"),
                                            FileMode.Create);
            var gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
            binaryFormatter.Serialize(gZipStream, items_);
            gZipStream.Close();
            gZipStream.Dispose();
            fileStream.Close();
            fileStream.Dispose();
        }

        public void Load() {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"))) {
                var binaryFormatter = new BinaryFormatter();

                var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"),
                                                FileMode.Open);
                var gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                items_ = (List<Items.Item>) binaryFormatter.Deserialize(gZipStream);
                foreach (Items.Item item in items_) {
                    item.UpdateData();
                }
                gZipStream.Close();
                gZipStream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }
    }
}