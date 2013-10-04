using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Button : IGameComponent
    {
        protected Rectangle locate_;
        public String Text;
        protected bool aimed_;
        protected IGameContainer Parent;

        protected readonly Texture2D whitepixel_;
        protected readonly SpriteFont font1_;
        protected TimeSpan lastPressed;
        protected bool firstPress = true;

        public bool Visible
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

        public Button(Vector2 p, string s, Texture2D wp, SpriteFont wf, IGameContainer pa)
        {
            whitepixel_ = wp;
            font1_ = wf;
            locate_.X = (int)p.X;
            locate_.Y = (int)p.Y;
            locate_.Height = 20;
            locate_.Width = (int)(font1_.MeasureString(s).X + 10);
            Text = s;
            Parent = pa;
            Parent.AddComponent(this);
            Visible = true;
        }

        public event EventHandler onPressed;

        protected void PressButton() {
            if (onPressed != null) {
                onPressed(this, null);
            }
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Visible) {
                Vector2 realpos = GetPosition();

                Color col = !aimed_ ? Settings.HudÑolor : Color.White;

                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                sb.DrawString(font1_, Text, realpos + new Vector2(5, 0), col);
            }
        }

        public virtual void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            if (Visible) {
                Vector2 realpos = GetPosition();
                Vector2 realdl = realpos;
                realdl.X += locate_.Width;
                realdl.Y += locate_.Height;

                if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh)
                {
                    aimed_ = true;
                    if (firstPress) {
                        if ((lms.LeftButton == ButtonState.Released ||
                             gt.TotalGameTime.TotalMilliseconds - lastPressed.TotalMilliseconds > 500) &&
                            ms.LeftButton == ButtonState.Pressed) {
                            PressButton();
                            lastPressed = gt.TotalGameTime;
                            firstPress = false;
                        }
                    }
                    else {
                        if ((lms.LeftButton == ButtonState.Released ||
                             gt.TotalGameTime.TotalMilliseconds - lastPressed.TotalMilliseconds > 100) &&
                            ms.LeftButton == ButtonState.Pressed)
                        {
                            PressButton();
                            lastPressed = gt.TotalGameTime;
                            firstPress = false;
                        }
                    }
                }
                else
                    aimed_ = false;

                if (lms.LeftButton == ButtonState.Released) firstPress = true;
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

        public float Width {
            get { return locate_.Width; }
        }

        public float Height {
            get { return locate_.Height; }
        }
    }
}