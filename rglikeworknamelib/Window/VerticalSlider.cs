using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class VerticalSlider : IGameComponent
    {
        private readonly IGameContainer Parent;

        private readonly Texture2D whitepixel_;

        public int Max = 100;
        public int Start = 0;
        public int Size = 10;
        public String Text;
        private Rectangle locate_;

        public event EventHandler OnMove;

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

            sb.Draw(whitepixel_, realpos, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(locate_.Width, 2),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, realpos, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(2, locate_.Height),
                    SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, Settings.HudÑolor,
                    0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, Settings.HudÑolor,
                    0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

            sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3 + (locate_.Height - 4 - 20) * ((float)Start / Max)) + Parent.GetPosition(), null,
                    Settings.HudÑolor, 0, Vector2.Zero,
                    new Vector2((locate_.Width - 4), (locate_.Height - 4 - 20) * ((float)Size / Max)),
                    SpriteEffects.None, 0);
        }

        private bool hooked;
        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            if (Visible) {
                var pos = GetPosition();
                if (ms.X >= pos.X && ms.Y >= pos.Y && ms.Y <= pos.X + Width && ms.Y <= pos.Y + Height) {
                    if (ms.LeftButton == ButtonState.Pressed && lms.LeftButton == ButtonState.Released) {
                        hooked = true;
                    }
                }
                if (ms.LeftButton == ButtonState.Released)
                {
                    hooked = false;
                }

                if (hooked)
                {
                    var cent = Max * (ms.Y - pos.Y) / locate_.Height;
                    Start = (int)(cent) - Size / 2;
                    if (Start < 0)
                    {
                        Start = 0;
                    }

                    if (Start + Size > Max)
                    {
                        Start = Max - Size;
                    }

                    if (OnMove != null) {
                        OnMove(this, EventArgs.Empty);
                    }

                    if (Parent is ListContainer)
                    {
                        var a = Parent as ListContainer;
                        a.FromI = Start;
                        a.RecalcContainer();
                        ((Window) a.parent_).parent_.Mopusehook = true;
                    }
                }
            }
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