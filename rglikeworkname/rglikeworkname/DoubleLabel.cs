using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib;
using rglikeworknamelib.Window;

namespace jarg {
    internal class DoubleLabel : Label {
        public string Text2;

        public DoubleLabel(Vector2 p, string s, Texture2D wp, SpriteFont wf, Color c, IGameContainer win) : base(p, s, wp, wf, c, win) {
        }

        public DoubleLabel(Vector2 p, string s, Texture2D wp, SpriteFont wf, IGameContainer win) : base(p, s, wp, wf, win) {
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Text != null && Visible)
            {
                if (isHudColored)
                {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.Hud—olor);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0),
                                      Color.LightBlue);
                    }
                }
                else
                {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, col_);
                    if (Text2 != null) {
                        sb.DrawString(font1_, Text2, Parent.GetPosition() + pos_ + new Vector2(Width, 0), Color.LightBlue);
                    }
                }
            }
        }
    }
}