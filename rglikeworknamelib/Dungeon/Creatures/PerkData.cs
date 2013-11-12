using System;
using LuaInterface;

namespace rglikeworknamelib.Creatures {
    public class PerkData {
        /// <summary>
        ///     ��������� �������� �����, ���� true, � ��������, ���� false, �� ���� ICreature
        /// </summary>
        public Action<bool, ICreature> AddRemove;

        public int Cost;
        public bool Initial;
        public string Name;

        public string Script;

        public PerkData(string s) {
            Name = s;
        }

        //TODO: move actions to lua
        public void RunSctipt(bool b, ICreature target, Lua lua) {
            // lua["target"]
            lua.DoString(Script);
        }
    }
}