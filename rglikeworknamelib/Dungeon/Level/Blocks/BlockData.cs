using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData {
        public string AfterDeathId;
        public string[] AlterMtex;

        public float Damage;
        public string Description;

        public float Hp;
        public bool IsDestructable, Flame, IsTransparent, IsWalkable, Wallmaker;
        public Color MMCol;
        public string MTex;
        public string Name;
        public SmartAction SmartAction;

        public List<DropGroup> ItemSpawn;

        public int StorageSlots;
        public int swide = 32;

        public string RandomMtexFromAlters() {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1) {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return MTex;
        }

        public static Rectangle GetSource(string s) {
            if (s == null) {
                return new Rectangle(0, 0, 0, 0);
            }
            int index = Atlases.BlockIndexes[s];
            return new Rectangle(index%32*32, index/32*32, 32, 32);
        }
    }
}