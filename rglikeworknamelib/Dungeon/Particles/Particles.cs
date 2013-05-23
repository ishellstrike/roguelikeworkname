using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Particles
{
    internal class Particle {
        internal Vector2 Pos;
        internal float Vel;
        internal float Angle, ASpeed;
        internal TimeSpan Life;

        internal int MTex;

        public Particle(Vector2 pos, int mTex) {
            Pos = pos;
            MTex = mTex;
        }

        public Particle(Vector2 pos, float vel, float an, float asp, int texn, TimeSpan ts) {
            Pos = pos;
            Vel = vel;
            ASpeed = asp;
            Angle = an;
            MTex = texn;
            Life = ts;
        }
    }

    public class ParticleSystem {
        private Random rnd = new Random();
        private Collection<Particle> particles_ = new Collection<Particle>();
        private SpriteBatch spriteBatch_;
        private Collection<Texture2D> partatlas;

        public ParticleSystem(SpriteBatch sb, Collection<Texture2D> pa) {
            spriteBatch_ = sb;
            partatlas = pa;
        }

        public void CreateParticle(Vector2 pos, float vel, float an, float asp, int texn, int lifeInSeconds) {
            particles_.Add(new Particle(pos, vel, an, asp, texn, new TimeSpan(0, 0, lifeInSeconds)));
        }

        private float GetRandorization( int rndPercent) {
            return (100 - rnd.Next(0, rndPercent))/100f;
        }

        public void CreateParticleWithRandomization(Vector2 pos, float vel, float an, float asp, int texn, int lifeInSeconds, int rndPercent)
        {
            particles_.Add(new Particle(pos, vel * GetRandorization(rndPercent), an, asp * GetRandorization(rndPercent), texn, new TimeSpan(0, 0, (int)(lifeInSeconds * GetRandorization(rndPercent)))));
        }

        public void Draw(GameTime gameTime, Vector2 camera) {
            for(int i=0;i<particles_.Count;i++) {
                    spriteBatch_.Draw(partatlas[particles_[i].MTex],
                                      particles_[i].Pos-camera,
                                      null, Color.White);
                }
            
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < particles_.Count; i++) {
                var a = particles_[i];
                particles_[i].Pos = new Vector2(a.Pos.X + a.Vel * (float)Math.Cos(a.Angle), particles_[i].Pos.Y + a.Vel * (float)Math.Sin(a.Angle));

                particles_[i].Angle += particles_[i].ASpeed;

                particles_[i].Life -= gameTime.ElapsedGameTime;

                if (particles_[i].Life <= TimeSpan.Zero) {
                    particles_.Remove(particles_[i]);
                    continue;
                }

                particles_[i].Vel /= 1.01f;
                particles_[i].ASpeed /= 1.01f;
            }
        }

        public int Count() {
            return particles_.Count;
        }
    }
}
