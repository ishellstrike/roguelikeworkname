using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class Item : IItem {
        public List<IBuff> Buffs { get; set; }
        public int Count { get; set; }
        public int Doses { get; set; }
        public List<ItemAction> Actions { get; set; }

        public string Id {
            get { return id_; }
            set { id_ = value; OnLoad(); }
        }

        public int Uid;
        [NonSerialized] internal ItemData data_;
        private string id_;

        public ItemData Data { get { return data_; }
        }

        //public Item(string i, int co) {
        //    Id = i;
        //    data_ = ItemDataBase.Data[i];
        //    Count = co;
        //    Uid = UniqueIds.GetNewItemId();
        //    Buffs = new List<IBuff>();
        //    Doses = Data.Doses;
        //    if (ItemDataBase.Data[i].Buff != null) {
        //        foreach (string buff in ItemDataBase.Data[i].Buff) {
        //            var a = (IBuff) Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
        //            a.Id = buff;
        //            Buffs.Add(a);
        //        }
        //    }
        //}

        //public Item(Item item) {
        //    Id = item.Id;
        //    data_ = ItemDataBase.Data[Id];
        //    Count = item.Count;
        //    Uid = item.Uid;
        //    Doses = Data.Doses;
        //    Buffs = new List<IBuff>();
        //    if (Data.Buff != null) {
        //        foreach (string buff in Data.Buff) {
        //            var a = (IBuff) Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
        //            a.Id = buff;
        //            Buffs.Add(a);
        //        }
        //    }
        //}

        public virtual void OnLoad() {
            data_ = ItemDataBase.Data[Id];
            Doses = Data.Doses;
            Buffs = new List<IBuff>();
            if (ItemDataBase.Data[Id].Buff != null)
            {
                foreach (string buff in ItemDataBase.Data[Id].Buff)
                {
                    var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
                    a.Id = buff;
                    Buffs.Add(a);
                }
            }
            Actions = new List<ItemAction>();
        }

        public override string ToString() {
            return Doses != 0
                       ? string.Format("{0} ({1})", ItemDataBase.Data[Id].Name, Doses)
                       : string.Format("{0} x{1}", ItemDataBase.Data[Id].Name, Count);
        }

        //public static bool operator == (Item a, Item b) {
        //    if (a == null || b == null) {
        //        return false;
        //    }
        //    return a.Uid == b.Uid;
        //}

        //public static bool operator !=(Item a, Item b) {
        //    return !(a == b);
        //}
    }

    public interface IItem {
        string Id { get; set; }
        int Count { get; set; }
        int Doses { get; set; }
        List<ItemAction> Actions { get; set; }
        ItemData Data { get; }
        List<IBuff> Buffs { get; set; }

        /// <summary>
        /// Reset data, buffs, doses, Class specific action init
        /// </summary>
        void OnLoad();
    }

    [Serializable]
    public class ItemAction {
        public string Name;
        public Action<Player> Action;
    }

    [Serializable]
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

    [Serializable]
    public class ItemCan : Item {
        public override void OnLoad()
        {
            base.OnLoad();
            Actions.Add(new ItemAction{Name = "Открыть банку", Action = OpenCan});
        }

        public void OpenCan(Player p) {
            if (p.Inventory.ContainsId("knife")) {
                p.Inventory.TryRemoveItem(Id, 1);
                p.Inventory.AddItem(ItemFactory.GetInstance(ItemDataBase.Data[Id].AfteruseId, 1));
            }
            else {
                EventLog.Add("Чтобы открывать банки вам нужен нож", Color.Yellow, LogEntityType.NoAmmoWeapon);
            }
        }
    }

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
            var a = Data.Weight;
            if (a > 10000) {
                EventLog.Add("Предмет слишком большой", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            if (a < 100) {
                EventLog.Add("Предмет слишком маленький", Color.Yellow, LogEntityType.NoAmmoWeapon);
                return;
            }
            p.Inventory.TryRemoveItem(Id, 1);
            var rnd = Settings.rnd;
            while (a >= 50)
            {
                var part = rnd.Next(2);
                switch (part)
                {
                    case 0:
                        a -= 50;
                        p.Inventory.AddItem(ItemFactory.GetInstance("brcloth", 1));
                        break;
                    case 1:
                        a -= 100;
                        p.Inventory.AddItem(ItemFactory.GetInstance("partcloth", 1));
                        break;
                }
            }
            EventLog.Add(string.Format("Вы успешно разорвали {0} на тряпки", Data.Name), Color.Yellow, LogEntityType.NoAmmoWeapon);
        }
    }
}