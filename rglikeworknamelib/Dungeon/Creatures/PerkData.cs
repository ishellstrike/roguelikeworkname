using System;
using LuaInterface;

namespace rglikeworknamelib.Creatures {
    public class PerkData {
        public PerkData(string s)
        {
            Name = s;
        }
        public string Name;
        
        public bool Initial;
        public int Cost;

        /// <summary>
        /// ��������� �������� �����, ���� true, � ��������, ���� false, �� ���� ICreature
        /// </summary>
        public Action<bool, ICreature> AddRemove;

        public string Script;

        public void RunSctipt(bool b, ICreature target, Lua lua) {
           // lua["target"]
            lua.DoString(Script);
        }
    }
}