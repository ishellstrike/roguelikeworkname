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
        public string Type;
        public Type TypeParsed;
        public float Height = 1;
        public SmartAction SmartAction;

        public List<DropGroup> ItemSpawn;

        public int swide =32;

        public string RandomMtexFromAlters() {
            if (AlterMtex != null && Settings.rnd.Next(-1, AlterMtex.Length) != -1)
            {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return MTex;
        }
    }
}