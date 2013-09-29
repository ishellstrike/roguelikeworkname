using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class StorageBlock : Block
    {
        public List<Item.Item> StoredItems = new List<Item.Item>();
    }
}