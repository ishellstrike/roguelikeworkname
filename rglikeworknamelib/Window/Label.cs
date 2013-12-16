using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Label : IGameComponent {
        protected readonly SpriteFont font1_;
        protected IGameContainer Parent;
        protected bool aimed_;

        private float calcHeight, calcWidth;
        public Color Color;
        protected bool isHudColored;
        protected Vector2 pos_;

        private string text_;

        public Label(Vector2 position, string text, Color c, IGameContainer win) {
            font1_ = win.font1_;
            pos_ = position;
            Text = text;
            Color = c;
            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public Label(Vector2 position, string text, IGameContainer win) {
            font1_ = win.font1_;
            pos_ = position;
            Text = text;
            Parent = win;
            Parent.AddComponent(this);
            isHudColored = true;
            Visible = true;
        }

        public virtual string Text {
            get { return text_; }
            set {
                text_ = value;
                text_ = text_.Trim('\n');
                Vector2 a = font1_.MeasureString(text_);
                calcHeight = a.Y;
                calcWidth = a.X;
            }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public virtual void Draw(SpriteBatch sb) {
            if (Text != null && Visible) {
                if (!aimed_ || OnLeftPressed == null) {
                    if (isHudColored) {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.HudÑolor);
                    }
                    else {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Color);
                    }
                }
                else {
                    if (isHudColored) {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.HudÑolor * 2f);
                    }
                    else {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Color * 2f);
                    }
                }
            }
        }

        public virtual void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks,
                                   bool mh) {
            if (Text != null) {
                var locate = new Rectangle((int) pos_.X, (int) pos_.Y, (int) calcWidth,
                                           (int) calcHeight);
                if (Visible) {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate.Width;
                    realdl.Y += locate.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh) {
                        aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed) {
                            OnOnLeftPressed(ms);
                        } 
                        else if (lms.RightButton == ButtonState.Released && ms.RightButton == ButtonState.Pressed)
                        {
                            OnOnRightPressed(ms);
                        }
                    }
                    else {
                        aimed_ = false;
                    }
                }
            }
        }

        public Vector2 GetPosition() {
            return pos_ + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            pos_ = pos;
        }

        public virtual float Width {
            get { return calcWidth; }
        }

        public float Height {
            get { return calcHeight; }
        }

        public event EventHandler<LabelPressEventArgs> OnLeftPressed;
        public event EventHandler<LabelPressEventArgs> OnRightPressed;

        protected virtual void OnOnRightPressed(MouseState ms) {
            if (OnRightPressed != null)
            {
                OnRightPressed(this, new LabelPressEventArgs { Ms = ms });
            }
        }

        private void OnOnLeftPressed(MouseState ms) {
            if (OnLeftPressed != null) {
                OnLeftPressed(this, new LabelPressEventArgs { Ms = ms });
            }
        }

        public void SetVisible(bool vis) {
            Visible = vis;
        }
    }

    public sealed class AchivementBox : IGameComponent
    {
        private readonly SpriteFont Font1;
        private readonly Texture2D image_;
        private IGameContainer Parent;
        //private bool aimed_;
        public Texture2D whitepixel_ { get; set; }
        public string Title, Description;
        public bool Completed;

        //private Color Color;
        private Rectangle location;

        public AchivementBox(Vector2 position, string text, Texture2D tex, IGameContainer win)
        {
            Font1 = win.font1_;
            location.X = (int) position.X;
            location.Y = (int) position.Y;
            Title = text;
            Description = "some desc";
            image_ = tex;
            Parent = win;
            Parent.AddComponent(this);
            location.Width = (int)Parent.Width - 50;
            location.Height = 96;
            Visible = true;
            whitepixel_ = Parent.whitepixel_;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb)
        {
            if (Visible)
            {
                Vector2 a = GetPosition();
                var b = new Rectangle((int)a.X, (int)a.Y, location.Width, location.Height);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudÑolor, 0, Vector2.Zero,
                        new Vector2(b.Width, 2), SpriteEffects.None, 0);

                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudÑolor, 0, Vector2.Zero,
                        new Vector2(2, b.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.Right, b.Y), null, Settings.HudÑolor, 0,
                        Vector2.Zero,
                        new Vector2(2, b.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Bottom), null, Settings.HudÑolor, 0,
                        Vector2.Zero,
                        new Vector2(b.Width + 2, 2), SpriteEffects.None, 0);

                sb.Draw(image_, new Vector2(a.X + 16, a.Y + 32), Color.White);
                sb.DrawString(Font1, Title, new Vector2(Width/2, 10) + a, Completed ? Color.GreenYellow : Color.Red);
                sb.DrawString(Font1, Description, new Vector2(64, 32) + a, Color.White);
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks,
                                   bool mh)
        {
                if (Visible)
                {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += location.Width;
                    realdl.Y += location.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh)
                    {
                        //aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
                        {
                            OnOnLeftPressed(ms);
                        }
                        else if (lms.RightButton == ButtonState.Released && ms.RightButton == ButtonState.Pressed)
                        {
                            OnOnRightPressed(ms);
                        }
                    }
                    else
                    {
                        //aimed_ = false;
                    }
                }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(location.X, location.Y) + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos)
        {
            location.X  = (int) pos.X;
            location.Y = (int)pos.Y;
        }

        public float Width
        {
            get { return location.Width; }
        }

        public float Height
        {
            get { return location.Height + 4; }
        }

        public event EventHandler<LabelPressEventArgs> OnLeftPressed;
        public event EventHandler<LabelPressEventArgs> OnRightPressed;

        private void OnOnRightPressed(MouseState ms)
        {
            if (OnRightPressed != null)
            {
                OnRightPressed(this, new LabelPressEventArgs { Ms = ms });
            }
        }

        private void OnOnLeftPressed(MouseState ms)
        {
            if (OnLeftPressed != null)
            {
                OnLeftPressed(this, new LabelPressEventArgs { Ms = ms });
            }
        }

        public void SetVisible(bool vis)
        {
            Visible = vis;
        }
    }

    public class LabelPressEventArgs : EventArgs {
        public MouseState Ms;
    }
}