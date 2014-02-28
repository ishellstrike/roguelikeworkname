using System;

namespace rglikeworknamelib.Dungeon.Level {
    class BlockFurnance : Block {
        public override void Update(TimeSpan ts) {
            foreach (var storedItem in StoredItems) {
                storedItem.DoubleTag = storedItem.DoubleTag + ts.TotalSeconds;
            }
            base.Update(ts);
        }
    }
}