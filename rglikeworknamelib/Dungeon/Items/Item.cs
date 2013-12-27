using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Items {
    [Serializable]
    public class Item {
        public List<IBuff> Buffs { get; set; }
        public int Count { get; set; }
        public int Doses { get; set; }

        public string Id {
            get { return id_; }
            set { id_ = value; OnLoad(); }
        }

        public int Uid;
        [NonSerialized]internal ItemData data_;
        private string id_;

        public ItemData Data { get { return data_; }
        }

        public virtual void OnLoad() {
            data_ = ItemDataBase.Instance.Data[Id];
            Doses = Data.Doses;
            Buffs = new List<IBuff>();
            if (ItemDataBase.Instance.Data[Id].Buff != null)
            {
                foreach (string buff in ItemDataBase.Instance.Data[Id].Buff)
                {
                    var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
                    a.Id = buff;
                    Buffs.Add(a);
                }
            }
        }

        public override string ToString() {
            return Doses != 0
                       ? string.Format("{0} ({1})", ItemDataBase.Instance.Data[Id].Name, Doses)
                       : string.Format("{0} x{1}", ItemDataBase.Instance.Data[Id].Name, Count);
        }
    }
}