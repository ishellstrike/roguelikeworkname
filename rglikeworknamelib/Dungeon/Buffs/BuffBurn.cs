using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Buffs {
    [Serializable]
    internal class BuffBurn : Buff {
        private TimeSpan ts;

        public override void Update(GameTime gt) {
            ts += gt.ElapsedGameTime;
            if (ts.TotalMilliseconds > 200) {
                var part = new Particle(new Vector2(Target.WorldPosition().X, Target.WorldPosition().Y) + new Vector2(16, 16), 2)
                {
                    Scale = 1.5f,
                    ScaleDelta = -0.8f,
                    Life = TimeSpan.FromMilliseconds(1500),
                    Angle = -3.14f/2f,
                    Rotation = Settings.rnd.Next()*6.28f,
                    Vel = 10,
                    RotationDelta = 1
                };
                ParticleSystem.AddParticle(part);
                ts = TimeSpan.Zero;
            }
            base.Update(gt);
        }
    }
}