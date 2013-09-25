using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Label : IGameComponent
    {
        protected Vector2 pos_;
        protected Color col_;
        protected IGameContainer Parent;
        protected bool isHudColored;
        public bool Visible { get; set; }

        private TimeSpan lastPressed;

        public object Tag { get; set; }
        protected bool aimed_;

        protected readonly SpriteFont font1_;

        private float calcHeight, calcWidth;

        private string text_;
        public virtual string Text {
            get { return text_; }
            set {
                text_ = value;
                text_ = text_.Trim('\n');
                var a = font1_.MeasureString(text_);
                calcHeight = a.Y;
                calcWidth = a.X; 
            }
        }

        public void SetPos(Vector2 pos) {
            pos_ = pos;
        }

        public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf, Color c, IGameContainer win)
        {
            font1_ = wf;
            pos_ = p;
            Text = s;
            col_ = c;
            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf, IGameContainer win)
        {
            font1_ = wf;
            pos_ = p;
            Text = s;
            Parent = win;
            Parent.AddComponent(this);
            isHudColored = true;
            Visible = true;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Text != null && Visible) {
                if (!aimed_ || onPressed == null) {
                    if (isHudColored) {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Settings.HudÑolor);
                    }
                    else {
                        sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, col_);
                    }
                }
                else {
                    sb.DrawString(font1_, Text, Parent.GetPosition() + pos_, Color.White);
                }
            }
        }

        public virtual void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            if (Text != null) {
                var locate_ = new Rectangle((int) pos_.X, (int) pos_.Y, (int) font1_.MeasureString(Text).X,
                                            (int) font1_.MeasureString(Text).Y);
                if (Visible) {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate_.Width;
                    realdl.Y += locate_.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh) {
                        aimed_ = true;
                        if ((lms.LeftButton == ButtonState.Released || lastPressed.TotalMilliseconds - gt.TotalGameTime.TotalMilliseconds > 500) && ms.LeftButton == ButtonState.Pressed) {
                            PressButton();
                            lastPressed = gt.TotalGameTime;
                        }
                    }
                    else
                        aimed_ = false;
                }
            }
        }

        public event EventHandler onPressed;

        void PressButton()
        {
            if (onPressed != null)
            {
                onPressed(this, null);
            }
        }

        public Vector2 GetPosition() {
            return pos_ + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            pos_ = pos;
        }

        public virtual float Width
        {
            get { return calcWidth; }
        }

        public float Height
        {
            get { return calcHeight; }
        }

        public void SetVisible(bool vis) {
            Visible = vis;
        }

        public int GetTag()
        {
            return 0;
        }
    }
}