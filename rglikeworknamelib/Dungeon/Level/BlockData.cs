using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData 
    {
        public int afterdeathId;

        public float damage;
        public string description;
        public bool isDestructable;

        public Type blockPrototype;
        public SmartAction smartAction;

        public float hp;
        public int texNo;
        public string name;

        public Color mmcol;

        public bool isWalkable;
        public bool isTransparent;

        public int storageSlots;

        public BlockData(int mtex) {
            texNo = mtex;
        }
        public BlockData() {
        }
    }
}