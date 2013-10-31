using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemData {
        /// <summary>
        ///     In 0.1 degree offset
        /// </summary>
        public int Accuracy;

        public string AfteruseId;

        public string Ammo;
        public string[] Buff;
        public string BulletParticle;
        public int Damage = 20;
        public string Description;
        public string Dress;
        public bool Hidden;

        public int Doses = 1;
        public int FireRate = 1000;

        public Color MMCol;
        public int MTex;
        public int Magazine;
        public string Name;
        public string Nameret;
        public int NutCal;
        public int NutH2O;

        public ItemType SType;
        public int Volume;
        public int Weight;
        public int hasHealth; //if has health, count became health
        public int stackNo;
    }
}