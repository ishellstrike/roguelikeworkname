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
            var ac = new ItemAction { Name = @"Включить\Выключить", Action = RadioOnOff };
            Actions.Add(ac);
        }

        private void RadioOnOff(Player p) {
            EventLog.Add("Радио включается", Color.White, LogEntityType.Default);
        }
    }
}