using System;
using Microsoft.Xna.Framework;
using jarg;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Items.Subclases {
    [Serializable]
    public class ItemCigarets : Item
    {

        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction {
                Name = "Выкурить",
                Action = Smoke
            });
        }

        public void Smoke(Player p)
        {
            if (p.Inventory.ContainsId("lighter1") || p.Inventory.ContainsId("lighter2")) {
                EventLog.Add(
                    string.Format("Вы выкурили сигарету "),
                    GlobalWorldLogic.CurrentTime, Color.Yellow, LogEntityType.Consume);
                foreach (IBuff buff in Buffs) {
                    buff.ApplyToTarget(p);
                }

                AchievementDataBase.Stat["sigause"].Count++;

                Doses--;
                if (Doses <= 0) {
                    p.Inventory.RemoveItem(this);
                }
            } else {
                EventLog.Add("Чтобы курить вам нужна зажигалка", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
    }
}