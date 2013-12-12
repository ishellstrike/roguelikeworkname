using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class VerticalProgressBar : IGameComponent {
        private readonly IGameContainer Parent;

        private readonly Texture2D whitepixel_;

        public int Max = 100;
        public int Progress = 0;
        public String Text;
        private Rectangle locate_;

        public VerticalProgressBar(Rectangle locate, IGameContainer container) {
            locate_ = locate;
            whitepixel_ = container.whitepixel_;
            Parent = container;
            Parent.AddComponent(this);
            Visible = true;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb) {
            Vector2 realpos = GetPosition();

            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width, 2),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3) + Parent.GetPosition(), null,
                    Settings.Hud—olor, 0, Vector2.Zero,
                    new Vector2((locate_.Width - 4), (locate_.Height - 4 - 20)*((float) Progress/Max)),
                    SpriteEffects.None, 0);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
        }

        public Vector2 GetPosition() {
            return new Vector2(locate_.X, locate_.Y) + Parent.GetPosition();
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

    public class VerticalSlider : IGameComponent
    {
        private readonly IGameContainer Parent;

        private readonly Texture2D whitepixel_;

        public int Max = 100;
        public int Start = 0;
        public int End = 10;
        public String Text;
        private Rectangle locate_;

        public VerticalSlider(Rectangle locate, IGameContainer container)
        {
            locate_ = locate;
            whitepixel_ = container.whitepixel_;
            Parent = container;
            Parent.AddComponent(this);
            Visible = true;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb)
        {
            Vector2 realpos = GetPosition();

            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(locate_.Width, 2),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.Hud—olor, 0, Vector2.Zero, new Vector2(2, locate_.Height),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, Settings.Hud—olor,
                    0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3 + (locate_.Height - 4 - 20) * ((float)Start / Max)) + Parent.GetPosition(), null,
                    Settings.Hud—olor, 0, Vector2.Zero,
                    new Vector2((locate_.Width - 4), (locate_.Height - 4 - 20) * ((float)(End - Start) / Max)),
                    SpriteEffects.None, 0);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
        }

        public Vector2 GetPosition()
        {
            return new Vector2(locate_.X, locate_.Y) + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos)
        {
            locate_.X = (int)pos.X;
            locate_.Y = (int)pos.Y;
        }

        public float Width
        {
            get { return locate_.Width; }
        }

        public float Height
        {
            get { return locate_.Height; }
        }
    }
}