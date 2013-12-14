using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    internal class BlockButningBochka : Block, ILightSource {
        private readonly Random rand = new Random();
        private TimeSpan sec_;

        public override bool IsActive
        {
            get
            {
                return true;
            }
        }

        public Color LightColor {
            get { return Color.LightYellow; }
        }

        public float LightPower {
            get { return 150; }
        }

        public float LightRange {
            get { return 200; }
        }

        public override void Update(TimeSpan ts, Vector2 vector2) {
            sec_ += ts;
            if (sec_.TotalSeconds > 0.2f && Lightness != Color.Black) {
                var p = new Vector2(vector2.X + 16 + rand.Next(-5, 5), vector2.Y + 8);
                var part = new Particle(p, 2) {
                    Scale = 1.5f,
                    ScaleDelta = -0.8f,
                    Life = TimeSpan.FromMilliseconds(1500),
                    Angle = -3.14f/2f,
                    Rotation = rand.Next()*6.28f,
                    Vel = 10,
                    RotationDelta = 1
                };
                ParticleSystem.AddParticle(part);
                sec_ = TimeSpan.Zero;
            }
        }
    }

    internal interface ILightSource {
        Color LightColor { get; }
        float LightPower { get; }
        float LightRange { get; }
    }
}