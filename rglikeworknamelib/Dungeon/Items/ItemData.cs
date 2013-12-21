using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items {
    public class ItemData {
        /// <summary>
        ///     In 0.1 degree offset
        /// </summary>
        public int Accuracy;

        public string AfteruseId;

        public string Ammo;
        
        public string[] Buff;
        
        public string BulletParticle;
        
        public int Damage ;
        
        public string Description;
        
        public string Dress;
        
        public bool Hidden;

        
        public int Doses ;
        
        public int FireRate ;

        
        public Color MMCol;
        
        public int MTex;
        
        public int Magazine;
        
        public string Name;
        
        public string Nameret;
        
        public string Using;
        
        public int NutCal;
        
        public int NutH2O;

        
        public string[] ActionsId ;

        public ItemType SortType;
        
        public int Volume;
        
        public int Weight;
        
        public int hasHealth; //if has health, count became health
        
        public int stackNo;

        
        public string SpawnGroup;
    }
}