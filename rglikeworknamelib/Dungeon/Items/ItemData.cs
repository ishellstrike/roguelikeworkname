using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Item {
    public class ItemData
    {
        public int stackNo;

        public int Weight;
        public string AfteruseId;

        public int hasHealth; //if has health, count became health

        public string Name;
        public string Nameret;
        public string Description;

        public int Doses;

        public int NutH2O, NutCal;

        public string[] Buff;

        public string Ammo;
        public int Magazine;
        public string BulletParticle;
        public int FireRate = 1000;
        public int Damage = 20;

        /// <summary>
        /// In 0.1 degree offset
        /// </summary>
        public int Accuracy;

        public int MTex;

        public Microsoft.Xna.Framework.Color MMCol;

        public int Volume;
        public ItemType SType;
    }
}