using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ProgressBar : IGameComponent {
        private readonly Window Parent;

        private readonly Color c1 = Color.Blue;
        private readonly Color c2 = Color.Red;
        private readonly SpriteFont font1_;
        private readonly Texture2D whitepixel_;

        public int Max = 100;
        public int Progress = 0;
        public String Text;
        private Rectangle locate_;

        public ProgressBar(Rectangle locate, string text, Window parent) {
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;

            locate_ = locate;
            Text = text;
            Parent = parent;
            Parent.AddComponent(this);
            Visible = true;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb) {
            Vector2 realpos = Parent.GetLocation() + GetPosition();

            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width, 2),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetLocation(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetLocation(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3) + Parent.GetLocation(), null,
                    Color.Lerp(c1, c2, (float) Progress/Max), 0, Vector2.Zero,
                    new Vector2((locate_.Width - 4)*((float) Progress/Max), locate_.Height - 4), SpriteEffects.None, 0);
            sb.DrawString(font1_, string.Format("{0}/{1}", Progress, Max), realpos + new Vector2(5, 0),
                          Settings.Hud—olor);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
        }

        public Vector2 GetPosition() {
            return new Vector2(locate_.X, locate_.Y);
        }

        public void SetPosition(Vector2 pos) {
            locate_.X = (int) pos.X;
            locate_.Y = (int) pos.Y;
        }

        public float Width {
            get { return locate_.Width; }
        }

        public float Height {
            get { return locate_.Height; }
        }
    }
}