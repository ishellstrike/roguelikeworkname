using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemData {
        public string AfteruseId;

        public string[] Buff;
        
        public string BulletParticle;


        public string Name;
        public string Nameret;
        public string Description;
        
        public string Dress;
        
        public bool Hidden;

        public string Require;
        public int Level;

        /// <summary>
        ///     In 0.1 degree offset
        /// </summary>
        public int Accuracy;
        public int Damage;
        public int Doses;
        /// <summary>
        /// In milliseconds between shoots
        /// </summary>
        public int FireRate;
        public int Magazine;
        public string Ammo;
        
        public Color MMCol;
        
        public int MTex;
        
        public string Using;
        
        public int NutCal;
        
        public int NutH2O;

        public string Type;
        public Type TypeParsed;

        public ItemType SortType;
        
        public int Volume;
        
        public int Weight;
        
        public string SpawnGroup;
        public string Id;
    }
}