using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Item {
    public class InventorySystem {
        private List<IItem> items_ = new List<IItem>();

        public int TotalWeight {
            get { return items_.Select(x => x.Data.Weight).Sum(); }
        }

        public int TotalVolume {
            get { return items_.Select(x => x.Data.Volume).Sum(); }
        }

        public void AddItem(IItem it) {
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

        public void RemoveItem(IItem it) {
            items_.Remove(it);
        }

        public bool ContainsItem(IItem it) {
            return items_.Contains(it);
        }

        public List<IItem> FilterByType(ItemType it) {
            return it == ItemType.Nothing ? items_ : items_.Where(item => item.Data.SType == it).ToList();
        }

        public void StackSimilar() {
            var a = new List<IItem>();

            foreach (IItem item in items_) {
                IItem it = a.FirstOrDefault(x => x.Id == item.Id && item.Doses != 0);
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

        public void AddItemRange(List<IItem> inContainer) {
            foreach (IItem item in inContainer) {
                AddItem(item);
            }
        }

        public IItem ContainsId(string itemData) {
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

        private bool CraftRequireCheck(string[] a, string[] b) {
            if (a == null) return true;
            return a.Select(t1 => items_.FirstOrDefault(x => x.Id == t1)).Where((t, i) => t != null && t.Count > int.Parse(b[i])).Any();
        }
    }
}