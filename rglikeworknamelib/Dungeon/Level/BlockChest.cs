using System.Collections.Generic;
using System.Text;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockChest : Block, IItemStorage {
        private List<Item> itemList_ = new List<Item>();
        public List<Item> ItemList {
            get { return itemList_; }
            set { itemList_ = value; }
        }

        public override string Serialize() {
            StringBuilder sb = new StringBuilder();
            foreach (var item in itemList_) {
                sb.Append(item.Id);
                sb.Append(',');
                sb.Append(item.Count);
                sb.Append(',');
                sb.Append(item.Doses);
                sb.Append('#');
            }
            return sb.ToString();
        }

        public override void Deserialize(string s) {
            var p  = s.Split('#');
            foreach (var s1 in p) {
                var pp = s.Split(',');
                var i = ItemFactory.GetInstance(pp[1], int.Parse(pp[2]));
                i.Doses = int.Parse(pp[3]);
                itemList_.Add(i);
            }
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