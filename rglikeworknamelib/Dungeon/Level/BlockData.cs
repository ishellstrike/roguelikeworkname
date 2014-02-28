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
        public string BlockScript;
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

        public static Vector2 GetSource(string s) {
            //server notex
            if (Atlases.Instance == null)
            {
                return new Vector2(0, 0);
            }
            int index = Atlases.Instance.MajorIndexes[s];
// ReSharper disable PossibleLossOfFraction
            return new Vector2((index%32*32f)/Atlases.Instance.MajorAtlas.Width, (index/32*32f)/Atlases.Instance.MajorAtlas.Height);
// ReSharper restore PossibleLossOfFraction
        }
    }
}