using System;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Effects;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    public class Buff : IBuff {
        public Creature Target { get; set; }
        public TimeSpan Expire { get; set; }
        public string Id { get; set; }
        public virtual bool RemoveFromTarget(Player p) {
            if(applied_) {
                Target = p;
                applied_ = false;
                return true;
            }
            return false;
        }

        public virtual bool ApplyToTarget(Player p) {
            if(!applied_) {
                Target = p;
                applied_ = true;
                return true;
            }
            return false;
        }

        internal bool applied_;
        public bool Applied {
            get { return applied_; }
        }
    }
}