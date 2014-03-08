using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockChest : Block, IItemStorage {
        private List<Item> itemList_ = new List<Item>();
        public List<Item> ItemList {
            get { return itemList_; }
            set { itemList_ = value; }
        }

        public bool RemoveItem(Item i) {
            if (ItemList.Contains(i))
            {
                ItemList.Remove(i);
                return true;
            }
            return false;
        }

        public bool AddItem(Item i) {
            ItemList.Add(i);
            return true;
        }
    }
}