using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Bullets {
    internal class Bullet {
        internal bool _gApply;
        internal int _mtex;
        internal Vector3 _position;
        internal int _type;

        internal Vector3 _velocity;

        public void Update(GameTime gameTime) {}

        public void Kill() {}

        public void Draw(GameTime gameTime) {}
    }
}