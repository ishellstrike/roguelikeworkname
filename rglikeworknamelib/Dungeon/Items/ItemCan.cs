using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCan : Item {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction{Name = "������� �����", Action = OpenCan});
        }

        public void OpenCan(Player p) {
            if (p.Inventory.ContainsId("knife")) {
                p.Inventory.TryRemoveItem(Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(ItemDataBase.Data[Id].AfteruseId, 1));
            }
            else {
                EventLog.Add("����� ��������� ����� ��� ����� ���", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
    }
}