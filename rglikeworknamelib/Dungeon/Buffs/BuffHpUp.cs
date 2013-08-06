using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Particles;

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

    class BuffBurn : Buff
    {
        TimeSpan ts = new TimeSpan();
        public override void Update(Microsoft.Xna.Framework.GameTime gt) {
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
        }
    }
}
