using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    class HealOnce : Buff
    {
        public override bool ApplyToTarget(Creature p) {
            p.hp_.Current += int.Parse(BuffDataBase.Data[Id].Value[0]);
            if (p.hp_.Current > p.hp_.Max) {
                p.hp_.Current = p.hp_.Max;
            }
            return base.ApplyToTarget(p);
        }
    }
}