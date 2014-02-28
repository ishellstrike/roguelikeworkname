using System;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockFurnance : Block {
        public override void Update(TimeSpan ts) {
            foreach (var storedItem in StoredItems) {
                if (storedItem.DoubleTag == null) storedItem.DoubleTag = 0.0;
                storedItem.DoubleTag = ((double) storedItem.DoubleTag) + ts.TotalSeconds;
            }
            base.Update(ts);
        }
    }
}