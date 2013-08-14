using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Dungeon.Item
{
    public class InventorySystem {
        public List<Item> items = new List<Item>();

        private ItemDataBase idb;

        public InventorySystem(ItemDataBase itemDataBase) {
            idb = itemDataBase;
        }

        public List<Item> FilterByType(ItemType it) {
            if (it == ItemType.Nothing) return items;
            var a = new List<Item>();

            foreach (var item in items) {
                if(idb.data[item.Id].stype == it) a.Add(item);
            }

            return a;
        }
    }
}
