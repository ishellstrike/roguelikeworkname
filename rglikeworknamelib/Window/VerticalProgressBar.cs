using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class VerticalProgressBar : IGameComponent
    {
        private Rectangle locate_;
        public String Text;
        private IGameContainer Parent;

        public bool Visible { get; set; }

        public object Tag { get; set; }

        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;

        private Color c1 = Color.Blue;
        private Color c2 = Color.Red;

        public int Progress = 0, Max = 100;

        public VerticalProgressBar(Rectangle r, string s, Texture2D whi, SpriteFont sf, IGameContainer pa)
        {
            locate_ = r;
            Text = s;
            whitepixel_ = whi;
            font1_ = sf;
            Parent = pa;
            Parent.AddComponent(this);
            Visible = true;
        }

        public void Draw(SpriteBatch sb)
        {
            Vector2 realpos = GetPosition();

            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3) + Parent.GetPosition(), null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2((locate_.Width - 4), (locate_.Height - 4) * ((float)Progress / Max)), SpriteEffects.None, 0);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, bool h)
        {

        }

        public Vector2 GetPosition()
        {
            return new Vector2(locate_.X, locate_.Y) +Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos)
        {
            locate_.X = (int)pos.X;
            locate_.Y = (int)pos.Y;
        }

        public float Width {
            get { return locate_.Width; }
        }

        public float Height {
            get { return locate_.Height; }
        }
    }
}