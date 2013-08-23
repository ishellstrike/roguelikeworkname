using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class BlockData 
    {
        public string AfterDeathId;

        public float Damage;
        public string Description;
        public bool IsDestructable;

        public Type BlockPrototype;
        public SmartAction SmartAction;

        public float Hp;
        public string MTex;
        public string Name;

        public Color MMCol;

        public bool IsWalkable;
        public bool IsTransparent;

        public int StorageSlots;

        public string[] AlterMtex;

        public string RandomMtexFromAlters()
        {
            if (AlterMtex == null || AlterMtex.Length == 0) {
                return MTex;
            }
            return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
        }

        public BlockData(string mtex) {
            MTex = mtex;
        }
        public BlockData() {
        }
    }
}