using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class ItemCloth : Item
    {
        private ItemAction[] actionlist_;

        public override ItemAction[] GetActionList
        {
            get { return actionlist_ ?? (actionlist_ = new[] { new ItemAction(DestroyCloth, "Разорвать на тряпки") }); }
        }
        public void DestroyCloth(Player p, Item target)
        {
            int weight = target.Data.Weight;
            if (weight > 10000)
            {
                EventLog.Add("Предмет слишком большой", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            if (weight < 100)
            {
                EventLog.Add("Предмет слишком маленький", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            p.Inventory.TryRemoveItem(target.Id, 1);
            int smallparts = 0;
            int bigparts = 0;
            Random rnd = Settings.rnd;
            while (weight >= 50)
            {
                int part = rnd.Next(2);
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
            string mess = string.Format("Вы успешно разорвали {0} на", target.Data.Name);
            if (bigparts > 0)
            {
                mess += string.Format(" {0} {1}", bigparts, Registry.Instance.Items["brcloth"].Name);
            }
            if (smallparts > 0)
            {
                mess += string.Format(" {0} {1}", smallparts, Registry.Instance.Items["partcloth"].Name);
            }
            EventLog.Add(mess, Color.Yellow, LogEntityType.NoAmmoWeapon);
        }
    }
}