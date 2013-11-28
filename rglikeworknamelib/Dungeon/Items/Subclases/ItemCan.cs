using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCan : Item {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction{Name = "Открыть банку", Action = OpenCan});
        }

        public void OpenCan(Player p) {
            if (p.Inventory.ContainsId("knife") || p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(ItemDataBase.Data[Id].AfteruseId, 1));
            }
            else {
                EventLog.Add("Чтобы открывать банки вам нужен нож или отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
    }
}