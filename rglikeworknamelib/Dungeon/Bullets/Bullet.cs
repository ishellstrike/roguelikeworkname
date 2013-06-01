using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Bullets {
    internal class Bullet {
        internal bool GApply;
        internal int Mtex;
        internal Vector3 Position;
        internal int Type;

        internal Vector3 Velocity;

        public void Update(GameTime gameTime) {}

        public void Kill() {}

        public void Draw(GameTime gameTime) {}
    }
}