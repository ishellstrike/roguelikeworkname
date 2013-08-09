using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData 
    {
        public int AfterDeathId;

        public float Damage;
        public string Description;
        public bool IsDestructable;

        public Type BlockPrototype;
        public SmartAction SmartAction;

        public float Hp;
        public int MTex;
        public string Name;

        public Color MMCol;

        public bool IsWalkable;
        public bool IsTransparent;

        public int StorageSlots;

        public int[] AlterMtex;

        public int RandomMtexFromAlters()
        {
            if (AlterMtex == null || AlterMtex.Length == 0) {
                return MTex;
            }
            return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
        }

        public BlockData(int mtex) {
            MTex = mtex;
        }
        public BlockData() {
        }
    }
}