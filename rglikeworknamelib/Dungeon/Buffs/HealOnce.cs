using System;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    internal class HealOnce : Buff {
        public override bool ApplyToTarget(Creature target) {
            target.hp_.Current += int.Parse(BuffDataBase.Data[Id].Value[0]);
            if (target.hp_.Current > target.hp_.Max) {
                target.hp_.Current = target.hp_.Max;
            }
            return base.ApplyToTarget(target);
        }
    }
}