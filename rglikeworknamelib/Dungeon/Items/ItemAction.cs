using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemAction {
        public string Name;
        public Action<Player, IItem> Action;

        public ItemAction(Action<Player, IItem> openCan, string s) {
            Name = s;
            Action = openCan;
        }
    }
}