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
        public int TexNo;
        public string Name;

        public Color MMCol;

        public bool IsWalkable;
        public bool IsTransparent;

        public int StorageSlots;

        public int Mtex;
        public int[] AlterMtex;

        public int RandomMtexFromAlters()
        {
            if (AlterMtex == null || AlterMtex.Length == 0) {
                return Mtex;
            }
            if (Settings.rnd.Next(1, 5) == 1) {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return Mtex;
        }

        public BlockData(int mtex) {
            TexNo = mtex;
        }
        public BlockData() {
        }
    }
}