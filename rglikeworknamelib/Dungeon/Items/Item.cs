using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Item
{
    [Serializable]
    public class Item
    {
        public string Id;
        [NonSerializedAttribute]public ItemData Data;
        public int Count;
        public int Uid;
        public List<IBuff> Buffs;

        public Item(string i, int co) {
            Id = i;
            Data = ItemDataBase.Data[i];
            Count = co;
            Uid = UniqueIds.GetNewItemId();
            Buffs = new List<IBuff>();
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
            Data = ItemDataBase.Data[Id];
            Count = item.Count;
            Uid = item.Uid;
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