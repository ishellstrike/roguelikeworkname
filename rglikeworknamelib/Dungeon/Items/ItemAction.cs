using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemAction {
        public string Name;
        public Action<Player> Action;
    }
}