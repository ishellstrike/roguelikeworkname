using System;
using jarg;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCigarets : Item {
        private ItemAction[] actions_;

        public override ItemAction[] GetActionList
        {
            get
            {
                return actions_ ?? (actions_ = new[] { new ItemAction(Smoke, "Выкурить") });
            }
        }

        public void Smoke(Player p, Item target) {
            if (p.Inventory.ContainsId("lighter1") || p.Inventory.ContainsId("lighter2")) {
                EventLog.Add(
                    string.Format("Вы выкурили сигарету "),
                    GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                foreach (IBuff buff in target.Buffs) {
                    buff.ApplyToTarget(p);
                }

                AchievementDataBase.Stat["sigause"].Count++;

                target.Doses--;
                if (target.Doses <= 0) {
                    p.Inventory.RemoveItem(target);
                }
            }
            else {
                EventLog.Add("Чтобы курить вам нужна зажигалка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
    }
}