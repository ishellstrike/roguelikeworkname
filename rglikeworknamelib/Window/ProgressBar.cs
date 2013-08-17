using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ProgressBar : IGameComponent {
        private Rectangle locate_;
        public String Text;
        private Window Parent;

        public bool Visible { get; set; }

        public object Tag { get; set; }

        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;

        private Color c1 = Color.Blue;
        private Color c2 = Color.Red;

        public int Progress = 0, Max = 100;

        public ProgressBar(Rectangle r, string s, Texture2D whi, SpriteFont sf, Window pa) {
            locate_ = r;
            Text = s;
            whitepixel_ = whi;
            font1_ = sf;
            Parent = pa;
            Parent.AddComponent(this);
            Visible = true;
        }

        public void Draw(SpriteBatch sb) {
            Vector2 realpos = Parent.GetLocation() + GetPosition();

            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetLocation(), null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetLocation(), null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3) + Parent.GetLocation(), null, Color.Lerp(c1, c2, (float)Progress / Max), 0, Vector2.Zero, new Vector2((locate_.Width - 4) * ((float)Progress / Max), locate_.Height - 4), SpriteEffects.None, 0);
            sb.DrawString(font1_, string.Format("{0}/{1}", Progress, Max), realpos + new Vector2(5, 0), Settings.Hud—olor);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms,bool h) {
                
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