using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Bullets {
    public class Bullet {
        internal int Mtex;

        internal Vector2 Pos;
        internal float Vel;
        internal float Angle, ASpeed;
        internal TimeSpan Life;
        private ParticleSystem ps;
        private Random rnd = new Random();

        internal Vector3 Velocity;

        public Bullet(Vector2 pos, float vel, float an, float asp, int texn, TimeSpan ts, ParticleSystem p) {
            Pos = pos;
            Vel = vel;
            ASpeed = asp;
            Angle = an;
            Mtex = texn;
            Life = ts;

            ps = p;

            ParticleSystem.CreateParticleWithRandomization(pos, 1, (float)rnd.NextDouble()*6f, 0, 1, 2, 10);
        }

        public void Kill() {}

        public void Update(GameTime gameTime) {

            if(rnd.Next(1,3) == 1)
                ParticleSystem.CreateParticleWithRandomization(Pos, 1, (float)rnd.NextDouble() * 6f, 0, 1, 2, 10);
        }
    }
}