using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class FloorData {
        public string Id;
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

        public string RandomMtexFromAlters()
        {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1)
            {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return MTex;
        }
    }
}