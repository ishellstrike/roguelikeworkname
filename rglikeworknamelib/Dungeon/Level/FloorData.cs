using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level
{
    public class FloorData{
        public FloorData(int a) {
            Mtex = a;
        }
        public FloorData() {
            
        }
        public int AfterdeathID;

        public float Damage;
        public string Description;
        public bool Destructable;

        public Color MMCol;

        public float HP;
        public int Mtex;
        public int[] AlterMtex;
        public string Name;

        public bool Walkable;
    }
}
