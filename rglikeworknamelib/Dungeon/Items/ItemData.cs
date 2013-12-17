using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Item {
    [DataContract]
    public class ItemData {
        /// <summary>
        ///     In 0.1 degree offset
        /// </summary>
        [DataMember]
        public int Accuracy{ get; set; }

        [DataMember]
        public string AfteruseId{ get; set; }

        [DataMember]
        public string Ammo{ get; set; }
        [DataMember]
        public string[] Buff{ get; set; }
        [DataMember]
        public string BulletParticle{ get; set; }
        [DataMember]
        public int Damage { get; set; }
        [DataMember]
        public string Description{ get; set; }
        [DataMember]
        public string Dress{ get; set; }
        [DataMember]
        public bool Hidden{ get; set; }

        [DataMember]
        public int Doses { get; set; }
        [DataMember]
        public int FireRate { get; set; }

        [DataMember]
        public Color MMCol{ get; set; }
        [DataMember]
        public int MTex{ get; set; }
        [DataMember]
        public int Magazine{ get; set; }
        [DataMember]
        public string Name{ get; set; }
        [DataMember]
        public string Nameret{ get; set; }
        [DataMember]
        public string Using{ get; set; }
        [DataMember]
        public int NutCal{ get; set; }
        [DataMember]
        public int NutH2O{ get; set; }

        [DataMember]
        public Type Prototype{ get; set; }

        [DataMember]
        public ItemType SortType{ get; set; }
        [DataMember]
        public int Volume{ get; set; }
        [DataMember]
        public int Weight{ get; set; }
        [DataMember]
        public int hasHealth{ get; set; } //if has health, count became health
        [DataMember]
        public int stackNo{ get; set; }

        [DataMember]
        public string SpawnGroup{ get; set; }
    }
}