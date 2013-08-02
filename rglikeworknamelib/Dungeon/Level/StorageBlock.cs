using System;
using System.Collections.Generic;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class StorageBlock : Block
    {
        public List<Item.Item> storedItems;
    }
}