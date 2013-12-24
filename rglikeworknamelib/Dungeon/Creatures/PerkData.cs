using System;

namespace rglikeworknamelib.Creatures {
    public class PerkData {
        /// <summary>
        ///     ��������� �������� �����, ���� true, � ��������, ���� false, �� ���� ICreature
        /// </summary>
        public Action<bool, Creature> AddRemove;

        public int Cost;
        public bool Initial;
        public string Name;

        public string Script;

        public PerkData(string s) {
            Name = s;
        }
    }
}