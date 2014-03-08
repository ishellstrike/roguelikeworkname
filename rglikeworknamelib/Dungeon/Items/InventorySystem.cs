using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Items {
    public class InventorySystem {
        private List<Item> items_ = new List<Item>();
        public bool Changed = false;

        public int TotalWeight {
            get { return items_.Select(x => x.Data.Weight).Sum(); }
        }

        public int TotalVolume {
            get { return items_.Select(x => x.Data.Volume).Sum(); }
        }

        public void AddItem(Item it) {
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
            Registry.StackSimilar(ref items_);
            Settings.InventoryUpdate = true;
        }

        public void RemoveItem(Item it) {
            items_.Remove(it);
            Settings.InventoryUpdate = true;
        }

        public bool ContainsItem(Item it) {
            return items_.Contains(it);
        }

        public List<Item> FilterByType(ItemType it) {
            return it == ItemType.Nothing ? items_ : items_.Where(item => item.Data.SortType == it).ToList();
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
                    player.EquipItem(selectedItem);
                    break;
                case ItemType.Medicine:
                case ItemType.Food:
                    if (player.EatItem(selectedItem)) {
                        EventLog.Add(
                            string.Format("{1} {0}", selectedItem.Data.Using ?? selectedItem.Data.Name,
                                          selectedItem.Data.SortType == ItemType.Medicine ? "Вы приняли" : "Вы употребили"), LogEntityType.Consume);
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
                            string.Format("Вы не можете съесть больше"), LogEntityType.Consume);
                    }
                    break;
            }
            Settings.InventoryUpdate = true;
        }

        public void AddItemRange(List<Item> inContainer) {
            foreach (Item item in inContainer) {
                AddItem(item);
            }
            Changed = true;
            Registry.StackSimilar(ref items_);
            Settings.InventoryUpdate = true;
        }

        public Item TryGetId(string itemData) {
            Settings.InventoryUpdate = true;
            return items_.FirstOrDefault(x => x.Id == itemData);
        }

        public bool ContainsId(string id) {
            return items_.FirstOrDefault(x => x.Id == id) != null;
        }

        public void Save() {
            var binaryFormatter = new BinaryFormatter();
            using (var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"),
                                                   FileMode.Create)) {
                using (var gZipStream = new GZipStream(fileStream, CompressionMode.Compress)) {
                    binaryFormatter.Serialize(gZipStream, items_);
                }
            }
        }

        public void Load() {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"))) {
                var binaryFormatter = new BinaryFormatter();

                using (var fileStream = new FileStream(Settings.GetWorldsDirectory() + string.Format("inventory.rlp"),
                                                       FileMode.Open)) {
                    using (var gZipStream = new GZipStream(fileStream, CompressionMode.Decompress)) {
                        items_ = (List<Item>) binaryFormatter.Deserialize(gZipStream);
                    }
                }
                foreach (Item item in items_) {
                    item.OnLoad();
                }
            }
        }

       // public void ExtractItem(string s,)

        public void Craft(ItemCraftData selectedCraft, Player crafter) {
            var able = CraftRequireCheck(selectedCraft);

            if (selectedCraft.Require != null) {
                foreach (var req in selectedCraft.Require) {
                    if (crafter.Abilities.List[req.Ability].XpLevel < req.Level) {
                        EventLog.Add("Недостаточный навык", LogEntityType.Consume);
                        return;
                    }
                }
            }

            if (able) {
                foreach (var inp in selectedCraft.Input) {
                    foreach (var alt in inp.Alters) {
                        var t = items_.FirstOrDefault(x => x.Id == alt.Id);
                        if (t != null) {
                            if (!alt.IsTool) {
                                if (t.Count > alt.Count) {
                                    t.Count -= alt.Count;
                                }
                                else if (t.Count == alt.Count) {
                                    items_.Remove(t);
                                }
                            }
                            break;
                        }
                    }
                }

                foreach (var inp in selectedCraft.Output) {
                    int te = Settings.rnd.Next(0, inp.Alters.Count);
                    items_.Add(ItemFactory.GetInstance(inp.Alters[te].Id, inp.Alters[te].Count));
                    EventLog.Add(string.Format("Вы создали {0}{1}", Registry.Instance.Items[inp.Alters[te].Id].Name, inp.Alters[te].Count > 1 ? " x" + inp.Alters[te].Count : string.Empty), LogEntityType.Consume);
                }

                Registry.StackSimilar(ref items_);
                Settings.InventoryUpdate = true;
            }
            else {
                EventLog.Add("Недостаточно компонентов", LogEntityType.Consume);
            }
        }

        private bool CraftRequireCheck(ItemCraftData a) {
            var tempInv = new List<Item>();
            tempInv.AddRange(items_);

            foreach (var inp in a.Input) {
                bool part = false;
                foreach (var alt in inp.Alters) {
                    var t = items_.FirstOrDefault(x => x.Id == alt.Id);
                    if (t != null) {
                        part = true;
                        if (!alt.IsTool) {
                            if (t.Count > alt.Count) {
                                t.Count -= alt.Count;
                            }
                            else if (t.Count == alt.Count) {
                                tempInv.Remove(t);
                            }
                            else {
                                return false;
                            }
                        }
                        break;
                    }
                }
                if (!part) return false;
            }
            return true;
        }
    }
}