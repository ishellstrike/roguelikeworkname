using System;
using System.Collections.Generic;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class StorageBlock : Block
    {
        public List<Item.Item> StoredItems = new List<Item.Item>();
    }
}