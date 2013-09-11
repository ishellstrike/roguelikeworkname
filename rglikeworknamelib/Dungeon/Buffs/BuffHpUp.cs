using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Particles;

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

    [Serializable]
    class BuffBurn : Buff
    {
        TimeSpan ts;
        public override void Update(GameTime gt) {
            ts += gt.ElapsedGameTime;
            if (ts.TotalMilliseconds > 200) {
                var part = new Particle(Target.WorldPosition() + new Vector2(16,16), 2) {
                    Scale = 1.5f,
                    ScaleDelta = -0.8f,
                    Life = TimeSpan.FromMilliseconds(1500),
                    Angle = -3.14f / 2f,
                    Rotation = Settings.rnd.Next() * 6.28f,
                    Vel = 10,
                    RotationDelta = 1
                };
                ParticleSystem.AddParticle(part);
                ts = TimeSpan.Zero;
            }
            base.Update(gt);
        }
    }

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
