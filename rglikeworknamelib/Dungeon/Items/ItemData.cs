using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items {
    [DataContract]
    public class ItemData {
        /// <summary>
        ///     In 0.1 degree offset
        /// </summary>
        [DataMember]
        public int Accuracy;

        [DataMember]
        public string AfteruseId;

        [DataMember]
        public string Ammo;
        [DataMember]
        public string[] Buff;
        [DataMember]
        public string BulletParticle;
        [DataMember]
        public int Damage ;
        [DataMember]
        public string Description;
        [DataMember]
        public string Dress;
        [DataMember]
        public bool Hidden;

        [DataMember]
        public int Doses ;
        [DataMember]
        public int FireRate ;

        [DataMember]
        public Color MMCol;
        [DataMember]
        public int MTex;
        [DataMember]
        public int Magazine;
        [DataMember]
        public string Name;
        [DataMember]
        public string Nameret;
        [DataMember]
        public string Using;
        [DataMember]
        public int NutCal;
        [DataMember]
        public int NutH2O;

        [DataMember]
        public string[] ActionsId ;

        [DataMember]
        public ItemType SortType;
        [DataMember]
        public int Volume;
        [DataMember]
        public int Weight;
        [DataMember]
        public int hasHealth; //if has health, count became health
        [DataMember]
        public int stackNo;

        [DataMember]
        public string SpawnGroup;
    }
}