using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class FloorData {
        public int AfterdeathID;
        public string[] AlterMtex;

        public float Damage;
        public string Description;
        public bool Destructable;

        public float HP;
        public Color MMCol;
        public string MTex;
        public string Name;

        public bool Walkable;

        public Rectangle RandomMtexFromAlters(ref string texstring) {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1) {
                texstring = AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            else {
                texstring = MTex;
            }
            return GetSource(texstring);
        }

        public static Rectangle GetSource(string s) {
            if (s == null) {
                return new Rectangle(0, 0, 0, 0);
            }
            int index = Atlases.FloorIndexes[s];
            return new Rectangle(index%32*32, index/32*32, 32, 32);
        }
    }
}