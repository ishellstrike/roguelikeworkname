using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    internal class ItemCan : Item {
        private ItemAction[] actionlist_;

        public override ItemAction[] GetActionList
        {
            get { return actionlist_ ?? (actionlist_ = new[] { new ItemAction(OpenCan, "������� �����") }); }
        }

        public void OpenCan(Player p, Item target)
        {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka")) {
                p.Inventory.TryRemoveItem(target.Id, 1);
                var afteruseId = ItemDataBase.Instance.Data[target.Id].AfteruseId;
                if (afteruseId != null) {
                    p.Inventory.AddItem(ItemFactory.GetInstance(afteruseId, 1));
                }
            }
            else
            {
                EventLog.Add("����� ��������� ����� ��� ����� ��� ��� ��������", Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
        }
    }
}