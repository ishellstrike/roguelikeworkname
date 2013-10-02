using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rglikeworknamelib.Creatures
{
    public class CreatureData {
        public string MTex;
        public string Name = "";
        public Type Prototype;
        public int Damage = 5;
        public int Speed = 50;

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int ReactionTime = 500;
    }
}
