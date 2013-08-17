using System;

namespace rglikeworknamelib.Dungeon.Item
{
    [Serializable]
    public class Item
    {
        public int Id;
        public int Count;
        public long Uid;

        public Item(int i, int co) {
            Id = i;
            Count = co;
            Uid = UniqueIds.GetNewItemId();
        }
    }
}