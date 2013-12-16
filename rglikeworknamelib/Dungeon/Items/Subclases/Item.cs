using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items.Subclases {
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
    }
}