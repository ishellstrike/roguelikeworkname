using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class CheckBox : IGameComponent {
        private readonly IGameContainer Parent;
        private readonly SpriteFont font1_;
        private readonly Vector2 mover = new Vector2(18, 0);
        private readonly Vector2 moverxy = new Vector2(3, 3);
        private readonly Vector2 movery = new Vector2(0, 13);
        private readonly Texture2D wp_;
        private float CalcHeight;
        private float CalcWidth;
        public bool Cheked = true;
        private bool aimed_;

        private Vector2 moverx = new Vector2(13, 0);
        private Vector2 pos_;
        private string text_;

        public CheckBox(Vector2 p, string s, IGameContainer parent) {
            wp_ = parent.whitepixel_;
            font1_ = parent.font1_;
            pos_ = p;
            Text = s;
            Parent = parent;
            Parent.AddComponent(this);
            Visible = true;
        }

        public string Text {
            get { return text_; }
            set {
                text_ = value;
                Vector2 a = font1_.MeasureString(text_);
                CalcHeight = a.Y;
                CalcWidth = a.X;
            }
        }

        public void Draw(SpriteBatch sb) {
            if (Text != null && Visible) {
                Vector2 vector2 = Parent.GetPosition() + pos_;
                if (!aimed_) {
                    sb.DrawString(font1_, Text, vector2 + mover, Settings.HudÑolor);
                    sb.Draw(wp_, vector2, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(15, 2),
                            SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(2, 15),
                            SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + moverx, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(2, 15),
                            SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + movery, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(15, 2),
                            SpriteEffects.None, 0);
                }
                else {
                    sb.DrawString(font1_, Text, vector2 + mover, Color.White);
                    sb.Draw(wp_, vector2, null, Color.White, 0, Vector2.Zero, new Vector2(15, 2), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2, null, Color.White, 0, Vector2.Zero, new Vector2(2, 15), SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + moverx, null, Color.White, 0, Vector2.Zero, new Vector2(2, 15),
                            SpriteEffects.None, 0);
                    sb.Draw(wp_, vector2 + movery, null, Color.White, 0, Vector2.Zero, new Vector2(15, 2),
                            SpriteEffects.None, 0);
                }

                if (Cheked) {
                    sb.Draw(wp_, vector2 + moverxy, null, Settings.HudÑolor, 0, Vector2.Zero, new Vector2(9, 9),
                            SpriteEffects.None, 0);
                }
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            if (Text != null && mh) {
                var locate = new Rectangle((int) (pos_.X - moverx.X), (int) pos_.Y, (int) (CalcWidth + moverx.X),
                                           (int) CalcHeight);
                if (Visible) {
                    Vector2 realpos = GetPosition();
                    Vector2 realdl = realpos;
                    realdl.X += locate.Width;
                    realdl.Y += locate.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y) {
                        aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed) {
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

        public float Width {
            get { return CalcWidth; }
        }

        public float Height {
            get { return CalcHeight; }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }
        public event EventHandler OnPressed;

        private void PressButton() {
            if (OnPressed != null) {
                OnPressed(this, null);
            }
        }
    }
}