using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items {
    public class InventorySystem {
        private List<IItem> items_ = new List<IItem>();
        public bool Changed = false;

        public int TotalWeight {
            get { return items_.Select(x => x.Data.Weight).Sum(); }
        }

        public int TotalVolume {
            get { return items_.Select(x => x.Data.Volume).Sum(); }
        }

        public void AddItem(IItem it) {
            if (it == null) return;
            items_.Add(it);
            switch (it.Data.SortType) {
                case ItemType.Ammo:
                    AchievementDataBase.Stat["ammototal"].Count += it.Count;
                    break;
                case ItemType.Gun:
                    AchievementDataBase.Stat["guntotal"].Count += it.Count;
                    break;
                case ItemType.Food:
                    AchievementDataBase.Stat["foodtotal"].Count += it.Count;
                    break;
            }
            Settings.InventoryUpdate = true;
        }

        public void RemoveItem(IItem it) {
            items_.Remove(it);
            Settings.InventoryUpdate = true;
        }

        public bool ContainsItem(IItem it) {
            return items_.Contains(it);
        }

        public List<IItem> FilterByType(ItemType it) {
            return it == ItemType.Nothing ? items_ : items_.Where(item => item.Data.SortType == it).ToList();
        }

        public void StackSimilar() {
            var a = new List<IItem>();

            foreach (IItem item in items_) {
                IItem it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses == 0);
                if (it != null) {
                    it.Count += item.Count;
                }
                else {
                    a.Add(item);
                }
            }

            //foreach (var item in items_) {
            //    FindById(a, item.Id).Count += item.Count;
            //}

            items_ = a;
            Settings.InventoryUpdate = true;
        }

        public bool TryRemoveItem(string id, int count) {
            var a = TryGetId(id);
            if (a.Count < count) return false;
            a.Count -= count;
            if (a.Count == 0) {
                items_.Remove(a);
            }

            return true;
        }

        public void UseItem(Item selectedItem, Player player) {
            if (selectedItem == null || player == null) {
                return;
            }
            switch (selectedItem.Data.SortType) {
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
                                          selectedItem.Data.SortType == ItemType.Medicine ? "Вы приняли" : "Вы употребили"),
                            GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                        foreach (IBuff buff in selectedItem.Buffs) {
                            buff.ApplyToTarget(player);
                        }

                        if (selectedItem.Data.SortType == ItemType.Medicine) {
                            AchievementDataBase.Stat["meduse"].Count++;
                        } else {
                            AchievementDataBase.Stat["fooduse"].Count++;
                        }

                        if (selectedItem.Doses > 0) {
                            selectedItem.Doses--;
                            if (selectedItem.Doses <= 0 && items_.Contains(selectedItem))
                            {
                                items_.Remove(selectedItem);
                            }
                        } else if (selectedItem.Count > 0) {
                            selectedItem.Count--;
                            if (selectedItem.Count <= 0 && items_.Contains(selectedItem))
                            {
                                items_.Remove(selectedItem);
                            }
                        }
                    }
                    else {
                        EventLog.Add(
                            string.Format("Вы не можете съесть больше"),
                            GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                    }
                    break;
            }
            Settings.InventoryUpdate = true;
        }

        public void AddItemRange(List<IItem> inContainer) {
            foreach (IItem item in inContainer) {
                AddItem(item);
            }
            Changed = true;
        }

        public IItem TryGetId(string itemData) {
            Settings.InventoryUpdate = true;
            return items_.FirstOrDefault(x => x.Id == itemData);
        }

        public bool ContainsId(string id) {
            return items_.FirstOrDefault(x => x.Id == id) != null;
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
                items_ = (List<IItem>) binaryFormatter.Deserialize(gZipStream);
                foreach (IItem item in items_) {
                    item.OnLoad();
                }
                gZipStream.Close();
                gZipStream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

       // public void ExtractItem(string s,)

        public void Craft(CraftData selectedCraft) {
            var a = CraftRequireCheck(selectedCraft.Input1, selectedCraft.Input1Count);
            var b = CraftRequireCheck(selectedCraft.Input2, selectedCraft.Input2Count);
            var c = CraftRequireCheck(selectedCraft.Input3, selectedCraft.Input3Count);
            var d = CraftRequireCheck(selectedCraft.Input4, selectedCraft.Input4Count);
        }

        private bool CraftRequireCheck(List<string> a, List<string> b) {
            if (a == null) return true;
            return a.Select(t1 => items_.FirstOrDefault(x => x.Id == t1)).Where((t, i) => t != null && t.Count > int.Parse(b[i])).Any();
        }
    }
}