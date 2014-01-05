using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemAction {
        public string Name;
        public dynamic Action;

        public ItemAction(dynamic openCan, string s) {
            Name = s;
            Action = openCan;
        }
    }
}