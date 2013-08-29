using System;
using rglikeworknamelib.Creatures;

namespace rglikeworknamelib.Dungeon.Effects {
    public class Buff : IBuff {
        public Creature Target { get; set; }
        public TimeSpan Expire { get; set; }
        public string Id { get; set; }
        public virtual bool RemoveFromTarget() {
            if(applied_) {

                applied_ = false;
                return true;
            }
            return false;
        }

        public virtual bool ApplyToTarget() {
            if(!applied_) {

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