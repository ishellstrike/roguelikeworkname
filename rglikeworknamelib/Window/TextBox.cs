using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class TextBox : IGameComponent
    {
        private Rectangle locate_;
        public String Text="";
        private bool aimed_;
        private IGameContainer Parent;

        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;
        private TimeSpan spam;
        private bool first, second;
        public event EventHandler onEnter;

        public TextBox(Vector2 p, int length, Texture2D wp, SpriteFont wf, IGameContainer pa)
        {
            whitepixel_ = wp;
            font1_ = wf;
            locate_.X = (int)p.X;
            locate_.Y = (int)p.Y;
            locate_.Height = 20;
            locate_.Width = length;
            Parent = pa;
            Parent.AddComponent(this);
            Visible = true;
        }

        public void Draw(SpriteBatch sb) {
            if (Visible)
            {
                Vector2 realpos = GetPosition();

                Color col = !aimed_ ? Settings.Hud�olor : Color.White;

                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                sb.DrawString(font1_, Text, realpos + new Vector2(5, 0) + Parent.GetPosition(), col);
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            spam += gt.ElapsedGameTime;
            if(ks.GetPressedKeys().Length == 0) {
                first = false;
                second = false;
            }

            foreach (var key in ks.GetPressedKeys()) {
                var a = key;
                if (!lks.IsKeyDown(a) || (spam.TotalMilliseconds >= 300 && !second) || (spam.TotalMilliseconds >= 100 && second))
                {
                    spam = TimeSpan.Zero;
                    if(first) {
                        second = true;
                    }
                    first = true;
                    switch (a) {
                        case Keys.Back:
                            if (Text.Length > 0) {
                                Text = Text.Remove(Text.Length - 1, 1);
                            }
                            break;
                        case Keys.Q:
                        case Keys.W:
                        case Keys.E:
                        case Keys.R:
                        case Keys.T:
                        case Keys.Y:
                        case Keys.U:
                        case Keys.I:
                        case Keys.O:
                        case Keys.P:
                        case Keys.A:
                        case Keys.S:
                        case Keys.D:
                        case Keys.F:
                        case Keys.G:
                        case Keys.H:
                        case Keys.J:
                        case Keys.K:
                        case Keys.L:
                        case Keys.Z:
                        case Keys.X:
                        case Keys.C:
                        case Keys.V:
                        case Keys.B:
                        case Keys.N:
                        case Keys.M:

                            if (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift)) {
                                Text += a.ToString();
                            }
                            else {
                                Text += a.ToString().ToLower();
                            }
                            break;
                        case Keys.D0:
                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                        case Keys.D4:
                        case Keys.D5:
                        case Keys.D6:
                        case Keys.D7:
                        case Keys.D8:
                        case Keys.D9:
                            Text += a.ToString()[1];
                            break;
                        case Keys.Space:
                            Text += " ";
                            break;
                        case Keys.OemComma:
                            Text += ",";
                            break;
                        case Keys.OemPeriod:
                            Text += ".";
                            break;
                        case Keys.Enter:
                            if(onEnter != null) {
                                onEnter(this, null);
                            }
                            break;

                    }
                }
            }
    }

        public Vector2 GetPosition() {
            return new Vector2(locate_.X, locate_.Y);
        }

        public void SetPosition(Vector2 pos) {
            locate_.X = (int)pos.X;
            locate_.Y = (int)pos.Y;
        }

        public float Width {
            get { return locate_.Width; }
        }

        public float Height {
            get { return locate_.Height; }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }
    }
}