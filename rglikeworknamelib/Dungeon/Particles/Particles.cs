using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Particles
{
    public class Particle
    {
        public Vector2 Pos;
        public float Vel;
        public float Angle, ASpeed;
        public TimeSpan Life;

        public float Scale = 1;
        public float ScaleDelta = 0;

        public float Transparency = 1;
        public float TransparencyDelta = 0;

        public Color Color = Color.White;

        internal int MTex;
        public float Rotation;
        public float RotationDelta;

        public Particle(Vector2 pos, int mTex)
        {
            Pos = pos;
            MTex = mTex;
        }

        public Particle(Vector2 pos, float vel, float an, float asp, int texn, TimeSpan ts)
        {
            Pos = pos;
            Vel = vel;
            ASpeed = asp;
            Angle = an;
            MTex = texn;
            Life = ts;
        }
    }

    public class ParticleSystem
    {
        private static Random rnd = new Random();
        private static Collection<Particle> particles_ = new Collection<Particle>();
        private static SpriteBatch spriteBatch_;
        private static Collection<Texture2D> partatlas_;

        public ParticleSystem(SpriteBatch sb, Collection<Texture2D> pa)
        {
            spriteBatch_ = sb;
            partatlas_ = pa;
        }

        public static void CreateParticle(Vector2 pos, float vel, float an, float asp, int texn, int lifeInSeconds)
        {
            particles_.Add(new Particle(pos, vel, an, asp, texn, TimeSpan.FromMilliseconds(lifeInSeconds)));
        }

        public static void AddParticle(Particle p) {
            particles_.Add(p);
        }

        private static float GetRandorization(int rndPercent)
        {
            return (100 - rnd.Next(0, rndPercent)) / 100f;
        }

        public static void CreateParticleWithRandomization(Vector2 pos, float vel, float an, float asp, int texn, int lifeInSeconds, int rndPercent)
        {
            particles_.Add(new Particle(pos, vel * GetRandorization(rndPercent), an, asp * GetRandorization(rndPercent), texn, TimeSpan.FromMilliseconds(lifeInSeconds * GetRandorization(rndPercent))));
        }

        public void Draw(GameTime gameTime, Vector2 camera)
        {
            for (int i = 0; i < particles_.Count; i++) {
                var particle = particles_[i];
                spriteBatch_.Draw(partatlas_[particle.MTex],
                                  particle.Pos - camera,
                                  null, particle.Color, particle.Rotation, new Vector2(partatlas_[particle.MTex].Height / 2, partatlas_[particle.MTex].Width/2), particle.Scale, SpriteEffects.None, 0);
            }
        }

        public void Update(GameTime gameTime) {
            var mul = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < particles_.Count; i++)
            {
                var particle = particles_[i];
                var a = particle;
                particle.Pos = new Vector2(a.Pos.X + a.Vel * (float)Math.Cos(a.Angle) * mul, particle.Pos.Y + a.Vel * (float)Math.Sin(a.Angle) * mul);

                particle.Angle += particle.ASpeed * mul;

                particle.Life -= gameTime.ElapsedGameTime;

                particle.Transparency += particle.TransparencyDelta * mul;
                particle.Scale += particle.ScaleDelta * mul;
                particle.Rotation += particle.RotationDelta * mul;

                if (particle.Life <= TimeSpan.Zero)
                {
                    particles_.Remove(particle);
                    continue;
                }
            }
        }

        public int Count()
        {
            return particles_.Count;
        }
    }
}