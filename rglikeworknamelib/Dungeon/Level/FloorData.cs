using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level
{
    public class FloorData{
        public int AfterdeathID;

        public float Damage;
        public string Description;
        public bool Destructable;

        public Color MMCol;

        public float HP;
        public string MTex;
        public string[] AlterMtex;
        public string Name;

        public bool Walkable;

        public Rectangle RandomMtexFromAlters(ref string texstring)
        {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1) {
                texstring = AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            } else {
                texstring = MTex;
            }
            return GetSource(texstring);
        }

        public static Rectangle GetSource(string s) {
            int index = Atlases.FloorIndexes[s];
            return new Rectangle(index % 32 * 32, index / 32 * 32, 32, 32);
        }

    }
}
