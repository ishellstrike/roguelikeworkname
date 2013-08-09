using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Creatures {
    public class Player : Creature {
        private readonly SpriteBatch sb_;
        public Texture2D Tex;
        public SpriteFont Font;

        public Player(SpriteBatch sb, Texture2D tex, SpriteFont font) {
            sb_ = sb;
            Font = font;
            Tex = tex;
            Position = new Vector2(1, 1);
        }

        public Player()
        {
        }

        public Vector2 CurrentActiveRoom { get; set; }

        public void Accelerate(Vector2 ac) {
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

            if (tpos.X < 0) a--;
            if (tpos.Y < 0) b--;
            if (tpos2.X < 0) c--;
            if (tpos2.Y < 0) d--;

            if (!gl.IsWalkable(a, b)) {
                Velocity.X = 0;
                if(bdb.Data[gl.GetBlock(a, b).Id].SmartAction == SmartAction.ActionOpenClose) {
                    gl.OpenCloseDoor(a,b);
                }
            }
            if (!gl.IsWalkable(c, d))
            {
                Velocity.Y = 0;
                if (bdb.Data[gl.GetBlock(c, d).Id].SmartAction == SmartAction.ActionOpenClose)
                {
                    gl.OpenCloseDoor(c, d);
                }
            }

            Position += Velocity * time * 20;

            if (time != 0) {
                Velocity /= Settings.H() / time;
            }
        }

        public void Draw(GameTime gt, Vector2 cam) {
            sb_.Draw(Tex, Position - cam, null, Color.White, 0, new Vector2(Tex.Width / 2, Tex.Height), 1,
                     SpriteEffects.None, 1);

            if (Settings.DebugInfo)
            {
                sb_.DrawString(Font, string.Format("{0}", Position), new Vector2(32 + Position.X - cam.X, -32 + Position.Y - cam.Y), Color.White);
            }
        }
    }
}