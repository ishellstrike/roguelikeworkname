using System;
using rglikeworknamelib.Creatures;
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

    internal class MoraleModifer : Buff {
        public override bool ApplyToTarget(Creature target) {
            if (target is Player) {
                var player = target as Player;
                player.Morale.Current += int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            return base.ApplyToTarget(target);
        }

        public override bool RemoveFromTarget(Creature target)
        {
            if (target is Player) {
                var player = target as Player;
                player.Morale.Current -= int.Parse(BuffDataBase.Data[Id].Value[0]);
            }
            return base.ApplyToTarget(target);
        }
    }

}