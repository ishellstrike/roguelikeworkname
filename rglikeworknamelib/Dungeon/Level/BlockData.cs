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
            texNo = mtex;
        }
        public BlockData() {
        }
    }
}