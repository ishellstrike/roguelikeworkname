using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Creatures;
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
            EventLog.Add(string.Format(s, p), LogEntityType.NoAmmoWeapon);
        }
    }

    [Serializable]
    public class Fuel : Item, IFuel {
         
    }

    public interface IFuel {
        
    }

    [Serializable]
    public class Lighter : Item, ILighter {
        private ItemAction[] actionlist_;

        public override ItemAction[] GetActionList
        {
            get { return actionlist_ ?? (actionlist_ = new[] { new ItemAction(LightItneract, "Поджечь") }); }
        }

        private void LightItneract(Player arg1, Item arg2) {
            Settings.InteractItem = new ItemAction(LightSome, "Поджечь (tehnical)");
        }

        private void LightSome(Player arg1, Item arg2) {
            var fuel = arg2 as IFuel;
            if (fuel != null) {
                EventLog.Add(string.Format("Вы подожгли {0}", arg2), LogEntityType.Consume);
                arg2.Modifer = ItemModifer.Goryashii;
            }
            else {
                EventLog.Add(string.Format("У вас не получилось разжечь {0}", arg2), LogEntityType.Consume);
            }
            Settings.InteractItem = null;
        }
    }

    interface ILighter {
        
    }
}