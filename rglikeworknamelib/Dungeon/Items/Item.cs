using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Item;

namespace rglikeworknamelib.Dungeon.Items
{
    [Serializable]
    public class Item
    {
        public string Id;
        [NonSerializedAttribute] private ItemData data_;
        public ItemData Data { get { return data_; } }
        public int Count;
        public int Doses;
        public int Uid;
        public List<IBuff> Buffs;

        public void UpdateData() {
            data_ = ItemDataBase.Data[Id];
        }

        public Item(string i, int co) {
            Id = i;
            data_ = ItemDataBase.Data[i];
            Count = co;
            Uid = UniqueIds.GetNewItemId();
            Buffs = new List<IBuff>();
            Doses = Data.Doses;
            if (ItemDataBase.Data[i].Buff != null)
            {
                foreach (var buff in ItemDataBase.Data[i].Buff)
                {
                    var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
                    a.Id = buff;
                    Buffs.Add(a);
                }
            }
        }

        public Item(Item item) {
            Id = item.Id;
            data_ = ItemDataBase.Data[Id];
            Count = item.Count;
            Uid = item.Uid;
            Doses = Data.Doses;
            Buffs = new List<IBuff>();
            if (Data.Buff != null)
            {
                foreach (var buff in Data.Buff) {
                    var a = (IBuff) Activator.CreateInstance(BuffDataBase.Data[buff].Prototype);
                    a.Id = buff;
                    Buffs.Add(a);
                }
            }
        }

        public override string ToString() {
            return Doses != 0 ? string.Format("{0} ({1})", ItemDataBase.Data[Id].Name, Doses) : string.Format("{0} x{1}", ItemDataBase.Data[Id].Name, Count);
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
}