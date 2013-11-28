using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items {
    public interface IItem {
        string Id { get; set; }
        int Count { get; set; }
        int Doses { get; set; }
        List<ItemAction> Actions { get; set; }
        ItemData Data { get; }
        List<IBuff> Buffs { get; set; }

        /// <summary>
        /// Reset data, buffs, doses, Class specific action init
        /// </summary>
        void OnLoad();
    }
}