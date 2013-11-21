using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    internal class BuffHpUp : Buff {
        public override bool ApplyToTarget(Creature target) {
            if (!applied_) {
                Target = target;
                Target.hp_.Max = Target.hp_.Max + int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            else {
                return false;
            }
            return base.ApplyToTarget(target);
        }

        public override bool RemoveFromTarget(Creature target) {
            if (applied_) {
                Target = target;
                Target.hp_.Max = Target.hp_.Max - int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            else {
                return false;
            }
            return base.RemoveFromTarget(target);
        }
    }

    [Serializable]
    internal class BuffCofeine : Buff
    {
        public override bool ApplyToTarget(Creature target)
        {
            if (!applied_)
            {
                Target = target;
                if (target is Player) {
                    var p = target as Player;
                    p.Sleep.Current -= int.Parse(BuffDataBase.Data[Id].Value[0]);
                }
            }
            else
            {
                return false;
            }
            return base.ApplyToTarget(target);
        }

        public override bool RemoveFromTarget(Creature target)
        {
            if (applied_)
            {
                Target = target;
            }
            else
            {
                return false;
            }
            return base.RemoveFromTarget(target);
        }
    }
}