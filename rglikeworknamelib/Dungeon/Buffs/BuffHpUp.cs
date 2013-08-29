using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Buffs
{
    [Serializable]
    class BuffHpUp : Buff {
        public override bool ApplyToTarget(Player p) {
            if (!applied_) {
                Target = p;
                Target.hp_.Max = Target.hp_.Max + BuffDataBase.Data[Id].Value1;

                applied_ = true;
                return true;
            }
            return false;
        }

        public override bool RemoveFromTarget(Player p)
        {
            if (applied_) {
                Target = p;
                Target.hp_.Max = Target.hp_.Max - BuffDataBase.Data[Id].Value1;

                applied_ = false;
                return true;
            }
            return false;
        }
    }
}
