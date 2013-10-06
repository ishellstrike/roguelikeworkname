using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ImageButton : Button {
        private Texture2D Im;
        private Color col = new Color(1,1,1,0.5f);
        public ImageButton(Vector2 position, string text, Texture2D im, Window ow) : base(position, text, ow) {
            Im = im;
            locate_.Height = im.Height;
            locate_.Width = im.Width;
        }
        public override void Draw(SpriteBatch sb) {
            if (Visible) {
                Vector2 realpos = GetPosition();

                Color col = !aimed_ ? Settings.HudÑolor : Color.White;

                Vector2 position = Parent.GetPosition();
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + position, null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + position, null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + position, null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + position, null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);
                sb.Draw(Im, new Vector2(locate_.X, locate_.Y) + position, Color.White);

                sb.DrawString(font1_, Text, realpos + new Vector2(5, 0), col);
            }
        }
        public override void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            if (Visible) {
                Vector2 realpos = GetPosition();
                Vector2 realdl = realpos;
                realdl.X += locate_.Width;
                realdl.Y += locate_.Height;

                if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y) {
                    aimed_ = true;
                    if (ms.LeftButton == ButtonState.Pressed && lms.LeftButton == ButtonState.Released) {
                        PressButton();
                    }
                } else
                    aimed_ = false;
            }
        }
    }
}