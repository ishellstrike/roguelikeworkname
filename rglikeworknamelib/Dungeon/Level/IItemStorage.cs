using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level {
    public interface IItemStorage {
        List<Item> ItemList { get; set; }
        bool RemoveItem(Item i);
        bool AddItem(Item i);
    }
}