using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCan : Item {
        private ItemAction[] actionlist_;

        public override ItemAction[] GetActionList
        {
            get { return actionlist_ ?? (actionlist_ = new[] { new ItemAction(OpenCan, "Открыть банку") }); }
        }

        public void OpenCan(Player p, Item target)
        {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka")) {
                p.Inventory.TryRemoveItem(target.Id, 1);
                var afteruseId = Registry.Instance.Items[target.Id].AfteruseId;
                if (afteruseId != null) {
                    p.Inventory.AddItem(ItemFactory.GetInstance(afteruseId, 1));
                }
            }
            else
            {
                EventLog.Add("Чтобы открывать банки вам нужен нож или отвертка", LogEntityType.NoAmmoWeapon);
            }
        }
    }
}