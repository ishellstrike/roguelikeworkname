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

        public Label(Vector2 position, string text, Color c, IGameContainer win)
        {
            font1_ = win.font1_;
            pos_ = position;
            Text = text;
            col_ = c;
            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public Label(Vector2 position, string text, IGameContainer win)
        {
            font1_ = win.font1_;
            pos_ = position;
            Text = text;
            Parent = win;
            Parent.AddComponent(this);
            isHudColored = true;
            Visible = true;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            if (Text != null && Visible) {
                if (!aimed_ || OnPressed == null) {
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
                var locate = new Rectangle((int) pos_.X, (int) pos_.Y, (int) calcWidth,
                                            (int) calcHeight);
                if (Visible) {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate.Width;
                    realdl.Y += locate.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh)
                    {
                        aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
                        {
                            PressButton();
                        }
                    }
                    else {
                        aimed_ = false;
                    }
                }
            }
        }

        public event EventHandler OnPressed;

        void PressButton()
        {
            if (OnPressed != null)
            {
                OnPressed(this, null);
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

    public class CheckBox : IGameComponent {
        private string text_;
        public string Text
        {
            get { return text_; }
            set
            {
                text_ = value;
                var a = font1_.MeasureString(text_);
                CalcHeight = a.Y;
                CalcWidth = a.X;
            }
        }
        private Vector2 pos_;
        private SpriteFont font1_;
        private bool aimed_;
        private IGameContainer Parent;
        private Texture2D wp_;
        public bool Cheked = true;
        public CheckBox(Vector2 p, string s, IGameContainer parent)
        {
            wp_ = parent.whitepixel_;
            font1_ = parent.font1_;
            pos_ = p;
            Text = s;
            Parent = parent;
            Parent.AddComponent(this);
            Visible = true;
        }

        private Vector2 mover = new Vector2(18,0);
        private Vector2 moverx = new Vector2(13, 0);
        private Vector2 movery = new Vector2(0, 13);
        private Vector2 moverxy = new Vector2(3, 3);
        public void Draw(SpriteBatch sb) {
            if (Text != null && Visible) {
                var vector2 = Parent.GetPosition() + pos_;
                if (!aimed_)
                {
                    sb.DrawString(font1_, Text, vector2 + mover, Settings.HudÑolor);
                    sb.Draw(wp_, vector2, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(15, 2), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(2, 15), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + moverx, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(2, 15), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + movery, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(15, 2), SpriteEffects.None, 0);
                }
                else
                {
                    sb.DrawString(font1_, Text, vector2 + mover, Color.White);
                    sb.Draw(wp_, vector2, null, Color.White, 0, Vector2.Zero, new Vector2(15, 2), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2, null, Color.White, 0, Vector2.Zero, new Vector2(2, 15), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + moverx, null, Color.White, 0, Vector2.Zero, new Vector2(2, 15), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + movery, null, Color.White, 0, Vector2.Zero, new Vector2(15, 2), SpriteEffects.None, 0);
                }

                if(Cheked) {
                    sb.Draw(wp_, vector2 + moverxy, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(9, 9), SpriteEffects.None, 0);
                }
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            if (Text != null) {
                var locate = new Rectangle((int) (pos_.X - moverx.X), (int) pos_.Y, (int) (CalcWidth + moverx.X),
                                            (int) CalcHeight);
                 if (Visible) {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate.Width;
                    realdl.Y += locate.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh)
                    {
                        aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
                        {
                            PressButton();
                            Cheked = !Cheked;
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

        public event EventHandler OnPressed;

        void PressButton()
        {
            if (OnPressed != null)
            {
                OnPressed(this, null);
            }
        }

        private float CalcWidth;
        public float Width {
            get { return CalcWidth; }
        }

        private float CalcHeight;
        public float Height {
            get { return CalcHeight; }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }
    }
}