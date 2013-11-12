using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class StorageBlock : Block {
        public List<IItem> StoredItems = new List<IItem>();
    }
}