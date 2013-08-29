using System;

namespace rglikeworknamelib.Dungeon.Item
{
    [Serializable]
    public class Item
    {
        public string Id;
        public int Count;
        public int Uid;

        public Item(string i, int co) {
            Id = i;
            Count = co;
            Uid = UniqueIds.GetNewItemId();
        }

        public Item(Item item) {
            Id = item.Id;
            Count = item.Count;
            Uid = item.Uid;
        }

        //public static bool operator == (Item a, Item b) {
        //    if (a == null || b == null) {
        //        return false;
        //    }
        //    return a.Uid == b.Uid;
        //}

        //public static bool operator !=(Item a, Item b) {
        //    return !(a == b);
        //}
    }
}