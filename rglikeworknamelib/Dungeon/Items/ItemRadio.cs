using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemRadio : Item {
        private ItemAction[] actions_;
        public override ItemAction[] GetActionList {
            get { return actions_ ?? (actions_ = new[] { new ItemAction(Disass, "Разобрать"), new ItemAction(RadioOnOff, "Включить\\выключить") }); }
        }

        private void Disass(Player p, Item target)
        {
            if (p.Inventory.ContainsId("otvertka"))
            {
                p.Inventory.TryRemoveItem(target.Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance("chipset", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("batery", 1));
                p.Inventory.AddItem(ItemFactory.GetInstance("smallvint", Settings.rnd.Next(5) + 10));

                EventLog.Add(string.Format("Вы успешно разбираете {0}", target.Data.Name), Color.Yellow,
                    LogEntityType.NoAmmoWeapon);
            }
            else
            {
                EventLog.Add("Чтобы разбирать электронику вам нужна отвертка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }

        private void RadioOnOff(Player p, Item target)
        {
            EventLog.Add("Радио включается", Color.White, LogEntityType.Default);
        }
    }
}