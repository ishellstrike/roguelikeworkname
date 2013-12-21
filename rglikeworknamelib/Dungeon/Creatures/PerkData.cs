using System;

namespace rglikeworknamelib.Creatures {
    public class PerkData {
        /// <summary>
        ///     Добавляет действие перка, если true, и вычитает, если false, на цель ICreature
        /// </summary>
        public Action<bool, ICreature> AddRemove;

        public int Cost;
        public bool Initial;
        public string Name;

        public string Script;

        public PerkData(string s) {
            Name = s;
        }
    }
}