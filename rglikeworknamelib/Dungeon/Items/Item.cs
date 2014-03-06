using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Level;

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

        public ItemModifer Modifer = ItemModifer.Nothing;

        [NonSerialized]internal ItemData data_;
        private string id_;

        public ItemData Data { get { return data_; }
        }

        public virtual void OnLoad() {
            data_ = Registry.Instance.Items[Id];
            Doses = Data.Doses;
            Buffs = new List<IBuff>();
            if (Registry.Instance.Items[Id].Buff != null)
            {
                foreach (string buff in Registry.Instance.Items[Id].Buff)
                {
                    var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
                    a.Id = buff;
                    Buffs.Add(a);
                }
            }
        }

        public override string ToString() {
            return Registry.Instance.ItemModifers[(int)Modifer].Name + " " + (Doses != 0
                       ? string.Format("{0} ({1})", Registry.Instance.Items[Id].Name, Doses)
                       : string.Format("{0} x{1}", Registry.Instance.Items[Id].Name, Count));
        }

        public virtual ItemAction[] GetActionList
        {
            get { return null; }
        }

        public static void Log(string s, params object[] p)
        {
            EventLog.Add(string.Format(s, p), Color.Yellow, LogEntityType.NoAmmoWeapon);
        }
    }
}