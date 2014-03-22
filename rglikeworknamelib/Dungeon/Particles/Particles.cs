using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Particles {
    [Serializable]
    public class Particle {
        public float ASpeed;
        public float Angle;
        public Color Color = Color.White;
        public TimeSpan Life;
        internal int MTex;
        public Vector2 Pos;
        public float Rotation;
        public float RotationDelta;

        public float Scale = 1;
        public float ScaleDelta = 0;

        public float Transparency = 1;
        public float TransparencyDelta = 0;
        public float Vel;

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
}