using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Image : IGameComponent {
        private readonly IGameContainer Parent;
        public Color col_;
        public Texture2D image;
        private Vector2 pos_;

        public Image(Vector2 position, Texture2D image, Color color, IGameContainer win) {
            pos_ = position;
            col_ = color;
            this.image = image;

            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public virtual void Draw(SpriteBatch sb) {
            sb.Draw(image, Parent.GetPosition() + pos_, col_);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
                var locate = new Rectangle((int)pos_.X, (int)pos_.Y, (int)image.Width,
                                           (int)image.Height);
                if (Visible)
                {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate.Width;
                    realdl.Y += locate.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y && mh)
                    {
                        if (ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed || ms.MiddleButton == ButtonState.Pressed) {
                            if (OnMouseDown != null) {
                                OnMouseDown(this, new MouseStateEventArgs(ms, lms));
                            }
                        }
                        else {
                            if (OnMouseUp != null)
                            {
                                OnMouseUp(this, new MouseStateEventArgs(ms, lms));
                            }
                        }

                        if (ms.X != lms.X || ms.Y != lms.Y) {
                            if (OnMouseMove != null)
                            {
                                OnMouseMove(this, new MouseStateEventArgs(ms, lms));
                            }
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

        public float Width {
            get { return image.Width; }
        }

        public float Height {
            get { return image.Height; }
        }

        public event EventHandler<MouseStateEventArgs> OnMouseDown;
        public event EventHandler<MouseStateEventArgs> OnMouseUp;
        public event EventHandler<MouseStateEventArgs> OnMouseMove;

        public Vector2 GetLocation() {
            return pos_;
        }

        public class MouseStateEventArgs : EventArgs {
            public MouseStateEventArgs(MouseState ms, MouseState lms) {
                Ms = ms;
                Lms = lms;
            }
            public MouseState Ms, Lms;
        }
    }
}