﻿namespace rglikeworknamelib.Dungeon.Creatures
{
    public class CreatureData {
        public int Damage;
        public string MTex;
        public string Name;

        /// <summary>
        ///     in milliseconds
        /// </summary>
        public int ReactionTime;

        public int Speed;

        public string BehaviorScript;

        public int Hp;

        public float Accuracy;

        public override string ToString() {
            return string.Format("{0} hp{1} dmg{2}",Name,Hp,Damage);
        }
    }
}