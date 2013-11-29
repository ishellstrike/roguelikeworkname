using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemWorkingRadio : Item {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction { Name = "�������� ��� ���������", Action = RadioOnOff });
            Actions.Add(new ItemAction{Name = "���������", Action = Disass});
        }

        private void Disass(Player p) {
            if (p.Inventory.ContainsId("otvertka")) {
                p.Inventory.TryRemoveItem(Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", 20));

                EventLog.Add(string.Format("�� ������� ���������� {0}", Data.Name), Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
            else {
                EventLog.Add("����� ��������� ����������� ��� ����� ��������", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
            
        }

        private void RadioOnOff(Player p) {
            EventLog.Add("����� ����������", Color.White, LogEntityType.Default);
        }
    }
}