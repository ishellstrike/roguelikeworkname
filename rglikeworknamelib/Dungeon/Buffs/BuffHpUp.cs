using System.Collections.Generic;
using System.Linq;
using System.Text;
using rglikeworknamelib.Dungeon.Buffs;

namespace rglikeworknamelib.Dungeon.Effects
{
    class BuffHpUp : Buff {
        public override bool ApplyToTarget() {
            if (!applied_) {
                Target.hp_.Max = Target.hp_.Max + BuffDataBase.Data[Id].Value1;

                applied_ = true;
                return true;
            }
            return false;
        }

        public override bool RemoveFromTarget()
        {
            if (applied_)
            {
                Target.hp_.Max = Target.hp_.Max - BuffDataBase.Data[Id].Value1;

                applied_ = false;
                return true;
            }
            return false;
        }
    }
}
