using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockFurnance : Block, IItemStorage {
        private TimeSpan sec;
        private List<Item> itemList_ = new List<Item>();

        public override void Update(TimeSpan ts, MapSector ms, Player p) {
            var secpos = new Vector3((ms.SectorOffsetX+0.5f)*MapSector.Rx, (ms.SectorOffsetY+0.5f)*MapSector.Ry, 0);
            foreach (var storedItem in ItemList)
            {
                    if (storedItem.Modifer == ItemModifer.Obuglivshiysa) {
                        sec += ts;
                        if (sec.TotalSeconds > 10) {
                            sec = new TimeSpan();

                            EventLog.AddLocated("вы слышите треск огня", p, secpos);
                            //
                            //fire!
                        }
                    }
                    else
                    {
                        var cook = storedItem as ICookable;
                        if (cook != null)
                        {
                            cook.GiveHeat(ts.TotalSeconds*40,p, secpos);
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