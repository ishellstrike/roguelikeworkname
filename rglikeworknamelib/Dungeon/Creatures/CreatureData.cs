using System;

namespace rglikeworknamelib.Creatures {
    public class CreatureData {
        public int Damage = 5;
        public string MTex;
        public string Name = "";
        public Type Prototype;

        /// <summary>
        ///     in milliseconds
        /// </summary>
        public int ReactionTime = 500;

        public int Speed = 50;

        public int Hp = 20;
    }
}