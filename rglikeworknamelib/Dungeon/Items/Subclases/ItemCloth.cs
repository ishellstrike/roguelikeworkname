using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCloth : Item
    {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction { Name = "Разорвать на тряпки", Action = DestroyCloth });
            
        }

        public void DestroyCloth(Player p)
        {
            var weight = Data.Weight;
            if (weight > 10000) {
                EventLog.Add("Предмет слишком большой", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            if (weight < 100) {
                EventLog.Add("Предмет слишком маленький", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            p.Inventory.TryRemoveItem(Id, 1);
            int smallparts = 0;
            int bigparts = 0;
            var rnd = Settings.rnd;
            while (weight >= 50)
            {
                var part = rnd.Next(2);
                switch (part)
                {
                    case 0:
                        weight -= 50;
                        p.Inventory.AddItem(ItemFactory.GetInstance("brcloth", 1));
                        smallparts++;
                        break;
                    case 1:
                        weight -= 100;
                        p.Inventory.AddItem(ItemFactory.GetInstance("partcloth", 1));
                        bigparts++;
                        break;
                }
            }
            string mess = string.Format("Вы успешно разорвали {0} на", Data.Name);
            if (bigparts > 0)
            {
                mess += string.Format(" {0} {1}", bigparts, ItemDataBase.Data["brcloth"].Name);
            }
            if (smallparts > 0) {
                mess += string.Format(" {0} {1}", smallparts, ItemDataBase.Data["partcloth"].Name);
            }
            EventLog.Add(mess, Color.Yellow, LogEntityType.NoAmmoWeapon);
        }
    }
}