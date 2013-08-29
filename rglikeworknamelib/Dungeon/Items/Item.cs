using System;
using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Item
{
    [Serializable]
    public class Item
    {
        public string Id;
        public int Count;
        public int Uid;
        public List<IBuff> Buffs;

        public Item(string i, int co) {
            Id = i;
            Count = co;
            Uid = UniqueIds.GetNewItemId();
            Buffs = new List<IBuff>();
            if (ItemDataBase.data[i].Buff1 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[i].Buff1].EffectPrototype);
                a.Id = ItemDataBase.data[i].Buff1;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[i].Buff2 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[i].Buff2].EffectPrototype);
                a.Id = ItemDataBase.data[i].Buff2;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[i].Buff3 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[i].Buff3].EffectPrototype);
                a.Id = ItemDataBase.data[i].Buff3;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[i].Buff4 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[i].Buff4].EffectPrototype);
                a.Id = ItemDataBase.data[i].Buff4;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[i].Buff5 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[i].Buff5].EffectPrototype);
                a.Id = ItemDataBase.data[i].Buff5;
                Buffs.Add(a);
            }
        }

        public Item(Item item) {
            Id = item.Id;
            Count = item.Count;
            Uid = item.Uid;
            Buffs = new List<IBuff>();
            if (ItemDataBase.data[Id].Buff1 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[Id].Buff1].EffectPrototype);
                a.Id = ItemDataBase.data[Id].Buff1;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[Id].Buff2 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[Id].Buff2].EffectPrototype);
                a.Id = ItemDataBase.data[Id].Buff2;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[Id].Buff3 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[Id].Buff3].EffectPrototype);
                a.Id = ItemDataBase.data[Id].Buff3;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[Id].Buff4 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[Id].Buff4].EffectPrototype);
                a.Id = ItemDataBase.data[Id].Buff4;
                Buffs.Add(a);
            }
            if (ItemDataBase.data[Id].Buff5 != null)
            {
                var a = (IBuff)Activator.CreateInstance(BuffDataBase.Data[ItemDataBase.data[Id].Buff5].EffectPrototype);
                a.Id = ItemDataBase.data[Id].Buff5;
                Buffs.Add(a);
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