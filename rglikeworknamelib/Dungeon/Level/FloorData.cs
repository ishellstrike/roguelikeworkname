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

        public string RandomMtexFromAlters()
        {
            if (AlterMtex != null && Settings.rnd.Next(1, 5) == 1)
            {
                return AlterMtex[Settings.rnd.Next(0, AlterMtex.Length)];
            }
            return MTex;
        }

        public static Vector2 GetSource(string s)
        {
            //server notex
            if (Atlases.Instance == null)
            {
                return new Vector2(0, 0);
            }
            int index = Atlases.Instance.MajorIndexes[s];
            return new Vector2((index % 32 * 32f) / Atlases.Instance.MajorAtlas.Width, (index / 32 * 32f) / Atlases.Instance.MajorAtlas.Height);
        }
    }
}