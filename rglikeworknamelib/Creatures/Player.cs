using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Creatures {
    public class Player : Creature {
        private readonly SpriteBatch sb_;
        public Texture2D Tex;
        public SpriteFont font_;

        public Player(SpriteBatch sb, Texture2D tex, SpriteFont font) {
            sb_ = sb;
            font_ = font;
            Tex = tex;
            Position = new Vector3(1, 1, 0);
        }

        public Player()
        {
        }

        public Vector2 CurrentActiveRoom { get; set; }

        public void Accelerate(Vector3 ac) {
            Velocity += ac;
        }

        public void Update(GameTime gt, GameLevel gl, BlockDataBase bdb) {
            base.Update(gt);

            var time = (float) gt.ElapsedGameTime.TotalSeconds;

            var tpos = Position; 
            tpos.X += Velocity.X;
            var tpos2 = Position;
            tpos2.Y += Velocity.Y;

            int a = (int) (tpos.X/32.0);
            int b = (int) (tpos.Y/32.0);

            int c = (int)(tpos2.X / 32.0);
            int d = (int)(tpos2.Y / 32.0);

            if (a < 0 || b < 0 || !gl.IsWalkable(a, b)) {
                Velocity.X = 0;
            }
            if (c < 0 || d < 0 || !gl.IsWalkable(c, d))
            {
                Velocity.Y = 0;
            }

            Position += Velocity * time * 20;

            if (time != 0) {
                Velocity /= Settings.H() / time;
            }
        }

        public void Draw(GameTime gt, Vector2 cam) {
            sb_.Draw(Tex, InScreenPosition - cam, null, Color.White, 0, new Vector2(Tex.Width / 2, Tex.Height), 1,
                     SpriteEffects.None, 1);

            if (Settings.DebugInfo)
            {
                sb_.DrawString(font_, string.Format("{0}", Position), new Vector2(32 + Position.X - cam.X, -32 + Position.Y - cam.Y), Color.White);
            }
        }
    }
}