using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Buffs
{
    [Serializable]
    class BuffHpUp : Buff {
        public override bool ApplyToTarget(Creature p) {
            if (!applied_) {
                Target = p;
                Target.hp_.Max = Target.hp_.Max + int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            else {
                return false;
            }
            return base.ApplyToTarget(p);
        }

        public override bool RemoveFromTarget(Creature p)
        {
            if (applied_) {
                Target = p;
                Target.hp_.Max = Target.hp_.Max - int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            else {
                return false;
            }
            return base.RemoveFromTarget(p);
        }
    }
}
