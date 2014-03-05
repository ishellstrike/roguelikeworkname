using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockFurnance : Block, IItemStorage {
        private string[] items_ = { "raw_meat", "carrot", "grusha", "banan", "apple", "steak", "goodsteak" };
        private TimeSpan sec;
        private List<Item> itemList_ = new List<Item>();

        public override void Update(TimeSpan ts, MapSector ms, Player p) {
            var secpos = new Vector3((ms.SectorOffsetX+0.5f)*MapSector.Rx, (ms.SectorOffsetY+0.5f)*MapSector.Ry, 0);
            foreach (var storedItem in ItemList)
            {
                if (items_.Contains(storedItem.Id)) {

                    if (storedItem.Modifer != ItemModifer.Obuglivshiysa) {
                        storedItem.DoubleTag = storedItem.DoubleTag + ts.TotalSeconds*40;
                    }
                    else {
                        sec += ts;
                        if (sec.TotalSeconds > 10) {
                            sec = new TimeSpan();

                            EventLog.AddLocated("вы слышите треск огня", p, secpos);
                            //
                            //fire!
                        }
                    }

                    if (storedItem.DoubleTag >= 100) {
                        storedItem.DoubleTag = 0;
                        switch (storedItem.Modifer) {
                            case ItemModifer.Razogretyi:
                                storedItem.Modifer = ItemModifer.Prigotovlenniy;
                                EventLog.AddLocated("вы чувствуете запах приготовленной пищи", p, secpos);
                                break;
                            case ItemModifer.Prigotovlenniy:
                                storedItem.Modifer = ItemModifer.Perejareniy;
                                break;
                            case ItemModifer.Perejareniy:
                                storedItem.Modifer = ItemModifer.Obuglivshiysa;
                                EventLog.AddLocated("вы чуствуете запах дыма", p, secpos);
                                break;
                            case ItemModifer.Nothing:
                                storedItem.Modifer = ItemModifer.Razogretyi;
                                break;
                        }
                    }
                }
            }
            base.Update(ts,ms,p);
        }

        public List<Item> ItemList {
            get { return itemList_; }
            set { itemList_ = value; }
        }

        public bool RemoveItem(Item i)
        {
            if (ItemList.Contains(i))
            {
                ItemList.Remove(i);
                return true;
            }
            return false;
        }

        public bool AddItem(Item i)
        {
            ItemList.Add(i);
            return true;
        }
    }
}