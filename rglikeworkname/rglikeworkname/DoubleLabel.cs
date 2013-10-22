using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib;
using rglikeworknamelib.Window;

namespace jarg {
    internal class DoubleLabel : Label {
        public string Text2;

        public DoubleLabel(Vector2 position, string text, Color c, IGameContainer win)
            : base(position, text, c, win) {
        }

        public DoubleLabel(Vector2 position, string text, IGameContainer win)
            : base(position, text, win) {
        }

        public override void Draw(SpriteBatch sb) {
            if (Text != null && Visible) {
                if (isHudColored) {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.Hud—olor);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0),
                                      Color.LightBlue);
                    }
                }
                else {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, col_);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0),
                                      Color.LightBlue);
                    }
                }
            }
        }
    }
}