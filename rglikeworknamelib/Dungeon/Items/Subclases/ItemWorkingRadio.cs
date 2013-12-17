using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items.Subclases {
    [Serializable]
    [DataContract]
    public class ItemWorkingRadio : Item {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction { Name = "Включить или выключить", Action = RadioOnOff });
            Actions.Add(new ItemAction{Name = "Разобрать", Action = Disass});
        }

        private void Disass(Player p) {
            if (p.Inventory.ContainsId("otvertka")) {
                p.Inventory.TryRemoveItem(Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", Settings.rnd.Next(5)+10));

                EventLog.Add(string.Format("Вы успешно разбираете {0}", Data.Name), Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
            else {
                EventLog.Add("Чтобы разбирать электронику вам нужна отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
            
        }

        private void RadioOnOff(Player p) {
            EventLog.Add("Радио включается", Color.White, LogEntityType.Default);
        }
    }
}