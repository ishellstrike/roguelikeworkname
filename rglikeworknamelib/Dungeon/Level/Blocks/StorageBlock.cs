using System;
using System.Collections.Generic;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class StorageBlock : Block
    {
        public List<Items.Item> StoredItems = new List<Items.Item>();
    }
}