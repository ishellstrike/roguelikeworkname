using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    class ItemBottle : Item {
        private ItemAction[] actionlist_;

        public override ItemAction[] GetActionList
        {
            get { return actionlist_ ?? (actionlist_ = new[] { new ItemAction(OpenBottle, "Открыть бутылку") }); }
        }
        public void OpenBottle(Player p, Item target)
        {
            p.Inventory.TryRemoveItem(target.Id, 1);
            p.Inventory.AddItem(ItemFactory.GetInstance(Registry.Instance.Items[target.Id].AfteruseId, 1));
        }
    }
}