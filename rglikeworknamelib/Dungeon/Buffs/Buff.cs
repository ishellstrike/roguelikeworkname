using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Effects;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    public class Buff : IBuff {
        public Creature Target { get; set; }
        public TimeSpan Expire { get; set; }

        public bool Expiring { get; set; }

        public string Id { get; set; }
        public virtual bool RemoveFromTarget(Creature target) {
            if(applied_) {
                Target = target;
                EventLog.Add(string.Format("Потерян эффект {0}", BuffDataBase.Data[Id].Name), GlobalWorldLogic.CurrentTime, Color.Orange, LogEntityType.Buff);
                applied_ = false;
                return true;
            }
            return false;
        }

        public virtual bool ApplyToTarget(Creature target) {
            if(!applied_) {
                Target = target;
                if (BuffDataBase.Data[Id].Duration != -1) {
                    Expire = new TimeSpan(0,0,BuffDataBase.Data[Id].Duration,0,0);
                    Expiring = true;
                } else {
                    Expiring = false;
                }
                EventLog.Add(string.Format("Получен эффект {0})", BuffDataBase.Data[Id].Name), GlobalWorldLogic.CurrentTime, Color.Orange, LogEntityType.Buff);
                applied_ = true;
                return true;
            }
            return false;
        }

        public virtual void Update(GameTime gt) {
            if(Expiring) {
                Expire -= GlobalWorldLogic.Elapse;
                if(Expire.TotalMilliseconds <= 0) {
                    RemoveFromTarget(Target);
                }
            }
        }

        internal bool applied_;
        public bool Applied {
            get { return applied_; }
        }
    }
}