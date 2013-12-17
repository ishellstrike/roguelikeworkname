using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;

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

        public int Doses = 0;
        public int FireRate = 1000;

        public Color MMCol;
        public int MTex;
        public int Magazine;
        public string Name;
        public string Nameret;
        public string Using;
        public int NutCal;
        public int NutH2O;

        public Type Prototype;

        public ItemType SortType;
        public int Volume;
        public int Weight;
        public int hasHealth; //if has health, count became health
        public int stackNo;

        public string SpawnGroup;
    }
}