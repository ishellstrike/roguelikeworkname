using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib
{
        public class WindowSystem : IGameWindowComponent
        {
            private static readonly List<Window> Windows = new List<Window>();
            Random rnd = new Random();
            public bool Mopusehook, Keyboardhook;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            public WindowSystem(Texture2D wp, SpriteFont f1) {
                whitepixel_ = wp;
                font1_ = f1;
            }

            //public Window New 


            public void Draw(SpriteBatch sb)
            {
                sb.Begin();
                foreach (var component in Windows) {
                    component.Draw(sb);
                }
                sb.End();
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms)
            {
                Mopusehook = false;
                Keyboardhook = false;

                if (Windows.Count != 0)
                    for (int i = Windows.Count - 1; i >= 0; i--) {
                        if (Windows.Count > i) {
                            var component = Windows[i];
                            if (component.Readytoclose) {
                                component.Visible = false;
                                Windows.Remove(component);
                                continue;
                            }
                            component.Update(gt, ms, lms);
                        }
                    }
            }

            public Vector2 GetLocation()
            {
                return Vector2.Zero;
            }

            internal void ToTop(Window win)
            {
                Windows.Remove(win);
                Windows.Insert(Windows.Count, win);
            }

            public void AddWindow(Window w) {
                Windows.Add(w);
            }

            public bool CloseTop() {
                for (int i = Windows.Count-1; i >= 0; i--) {
                    var window = Windows[i];
                    if (window.Closable && window.Visible) {
                        window.Visible = false;
                        return true;
                    }
                }
                return false;
            }
        }

        public class Window : IGameWindowComponent
        {
            internal List<IGameWindowComponent> Components = new List<IGameWindowComponent>();

            private readonly WindowSystem parent_;

            public Rectangle Locate;
            private readonly Color backtransparent_;

            private Button closeButton_;

            public bool Readytoclose;

            private bool closable_ = true;
            public bool Closable {
                get { return closable_; }
                set
                {
                    closable_ = value;
                    if (closeButton_ != null) {
                        closeButton_.Visible = value;
                    }
                }
            }
            public bool Moveable = true;
            public bool Visible = true;
            public bool NoBorder;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            public string Name;

            /// <summary>
            /// Create window in exact position whith exact sise
            /// </summary>
            /// <param name="location"></param>
            /// <param name="caption"></param>
            /// <param name="closeable"></param>
            /// <param name="wp"></param>
            /// <param name="wf"></param>
            /// <param name="parent"></param>
            public Window(Rectangle location, string caption, bool closeable, Texture2D wp, SpriteFont wf, WindowSystem parent) {
                whitepixel_ = wp;
                font1_ = wf;
                parent_ = parent;

                Name = caption;
                Locate = location;
                backtransparent_ = Color.Black;
                backtransparent_.A = 220;
                Closable = closeable;
                
                closeButton_ = new Button(new Vector2(Locate.Width - 22, -22), "x", whitepixel_, font1_, this);
                closeButton_.onPressed += closeButton__onPressed;
                if (!Closable) {
                    closeButton_.Visible = false;
                }

                parent.AddWindow(this);
            }

            /// <summary>
            /// Create window at center of screen
            /// </summary>
            /// <param name="size"></param>
            /// <param name="caption"></param>
            /// <param name="closeable"></param>
            /// <param name="wp"></param>
            /// <param name="wf"></param>
            /// <param name="parent"></param>
            public Window(Vector2 size, string caption, bool closeable, Texture2D wp, SpriteFont wf, WindowSystem parent)
            {
                whitepixel_ = wp;
                font1_ = wf;
                parent_ = parent;

                Name = caption;
                Locate = new Rectangle((int)(Settings.Resolution.X/2 - size.X/2),(int)(Settings.Resolution.Y/2 - size.Y/2), (int)size.X, (int)size.Y);
                backtransparent_ = Color.Black;
                backtransparent_.A = 220;
                Closable = closeable;

                closeButton_ = new Button(new Vector2(Locate.Width - 22, -22), "x", whitepixel_, font1_, this);
                closeButton_.onPressed += closeButton__onPressed;
                if (!Closable)
                {
                    closeButton_.Visible = false;
                }

                parent.AddWindow(this);
            }

            void closeButton__onPressed(object sender, EventArgs e)
            {
                Close();
            }

            public void AddComponent(IGameWindowComponent component) {
                Components.Add(component);
            }

            public void Close() {
                Visible = false;
            }

            public void Draw(SpriteBatch sb)
            {
                if (Visible)
                {
                    if(!NoBorder)
                    {
                        sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, backtransparent_, 0, Vector2.Zero,
                                new Vector2(Locate.Width, Locate.Height), SpriteEffects.None, 0);


                        sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero,
                                new Vector2(Locate.Width, 2), SpriteEffects.None, 0);
                        sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y + 20), null, Settings.HudСolor, 0,
                                Vector2.Zero,
                                new Vector2(Locate.Width, 2), SpriteEffects.None, 0);

                        Vector2 textpos = new Vector2(Locate.X + Locate.Width/2 - 11 - font1_.MeasureString(Name).X/2,
                                                      Locate.Y);

                        sb.DrawString(font1_, Name, textpos, Settings.HudСolor);


                        sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero,
                                new Vector2(2, Locate.Height), SpriteEffects.None, 0);
                        sb.Draw(whitepixel_, new Vector2(Locate.Right, Locate.Y), null, Settings.HudСolor, 0,
                                Vector2.Zero,
                                new Vector2(2, Locate.Height + 2), SpriteEffects.None, 0);
                        sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Bottom), null, Settings.HudСolor, 0,
                                Vector2.Zero,
                                new Vector2(Locate.Width + 2, 2), SpriteEffects.None, 0);
                    }

                    for (int i = 0; i < Components.Count; i++)
                    {
                        var component = Components[i];
                        component.Draw(sb);
                    }
                }
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms)
            {
                if (Visible) {
                    if (parent_.Mopusehook == false && lms.X >= Locate.Left && lms.Y >= Locate.Top &&
                        lms.X <= Locate.Right && lms.Y <= Locate.Bottom) {
                        parent_.Mopusehook = true;

                        if (lms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {

                            parent_.ToTop(this);
                        }

                        if (Moveable && lms.LeftButton == ButtonState.Pressed && lms.Y <= Locate.Top + 20) {
                            Locate.X += (int) (ms.X - lms.X);
                            Locate.Y += (int) (ms.Y - lms.Y);
                        }

                        for (int i = Components.Count - 1; i >= 0; i--) {
                            var component = Components[i];
                            component.Update(gt, ms, lms);
                        }
                    }
                }
            }

            public Vector2 GetLocation()
            {
                return new Vector2(Locate.X + 2, Locate.Y + 22);
            }

            public void OnTop() {
                parent_.ToTop(this);
            }
        }

        public class Label : IGameWindowComponent
        {
            private readonly Vector2 pos_;
            public String Text;
            private Color col_;
            private Window Parent;
            private bool isHudColored;

            private readonly SpriteFont font1_;

            public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf, Color c, Window win)
            {
                font1_ = wf;
                pos_ = p;
                Text = s;
                col_ = c;
                Parent = win;
                Parent.AddComponent(this);
            }

            public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf, Window win)
            {
                font1_ = wf;
                pos_ = p;
                Text = s;
                Parent = win;
                Parent.AddComponent(this);
                isHudColored = true;
            }

            public void Draw(SpriteBatch sb)
            {
                if(isHudColored) {
                    sb.DrawString(font1_, Text, Parent.GetLocation() + pos_, Settings.HudСolor);
                } else {
                    sb.DrawString(font1_, Text, Parent.GetLocation() + pos_, col_);
                }
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms)
            {
                
            }

            public Vector2 GetLocation()
            {
                return pos_;
            }
        }

        public class Image : IGameWindowComponent
        {
            private readonly Vector2 pos_;
            public String Text;
            public Color col_;
            public Texture2D image;
            private Window Parent;

            private readonly SpriteFont font1_;

            public Image(Vector2 p, Texture2D im, Color c, Window win)
            {
                pos_ = p;
                col_ = c;
                image = im;

                Parent = win;
                Parent.AddComponent(this);
            }

            public virtual void Draw(SpriteBatch sb)
            {
                sb.Draw(image, Parent.GetLocation() + pos_, col_);
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms)
            {

            }

            public Vector2 GetLocation()
            {
                return pos_;
            }
        }

        public class ProgressBar : IGameWindowComponent {
            private Rectangle locate_;
            public String Text;
            private Window Parent;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            private Color c1 = Color.Blue;
            private Color c2 = Color.Red;

            public int Progress = 0, Max = 100;

            public ProgressBar(Rectangle r, string s, Texture2D whi, SpriteFont sf, Window pa) {
                locate_ = r;
                Text = s;
                whitepixel_ = whi;
                font1_ = sf;
                Parent = pa;
                Parent.AddComponent(this);
            }

            public void Draw(SpriteBatch sb) {
                Vector2 realpos = Parent.GetLocation() + GetLocation();

                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetLocation(), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetLocation(), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetLocation(), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetLocation(), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                sb.Draw(whitepixel_, new Vector2(locate_.X + 3, locate_.Y + 3) + Parent.GetLocation(), null, Color.Lerp(c1, c2, (float)Progress / Max), 0, Vector2.Zero, new Vector2((locate_.Width - 4) * ((float)Progress / Max), locate_.Height - 4), SpriteEffects.None, 0);
                sb.DrawString(font1_, string.Format("{0}/{1}", Progress, Max), realpos + new Vector2(5, 0), Settings.HudСolor);
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms) {
                
            }

            public Vector2 GetLocation() {
                return new Vector2(locate_.X, locate_.Y);
            }
        }

        public class Button : IGameWindowComponent
        {
            private Rectangle locate_;
            public String Text;
            private bool aimed_;
            private Window Parent;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;
            public bool Visible = true;

            public Button(Vector2 p, string s, Texture2D wp, SpriteFont wf, Window pa)
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
            }

            public event EventHandler onPressed;

            void PressButton() {
                if (onPressed != null) {
                    onPressed(this, null);
                }
            }

            public void Draw(SpriteBatch sb)
            {
                if (Visible) {
                    Vector2 realpos = Parent.GetLocation() + GetLocation();

                    Color col = !aimed_ ? Settings.HudСolor : Color.White;

                    sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetLocation(), null, col, 0,
                            Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                    sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetLocation(), null, col, 0,
                            Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                    sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetLocation(), null, col, 0,
                            Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                    sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetLocation(), null, col, 0,
                            Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                    sb.DrawString(font1_, Text, realpos + new Vector2(5, 0), col);
                }
            }

            public void Update(GameTime gt, MouseState ms, MouseState lms)
            {
                if (Visible) {
                    Vector2 realpos = Parent.GetLocation() + GetLocation();
                    Vector2 realdl = realpos;
                    realdl.X += locate_.Width;
                    realdl.Y += locate_.Height;

                    if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y) {
                        aimed_ = true;
                        if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed) {
                            PressButton();
                        }
                    }
                    else
                        aimed_ = false;
                }
            }

            public Vector2 GetLocation()
            {
                return new Vector2(locate_.X, locate_.Y);
            }
        }

        public class ImageButton : Button {
            private Texture2D Im;
            public ImageButton(Vector2 p, string s, ComponentAction ac, Texture2D wp, Texture2D im, SpriteFont wf, Window ow) : base(p, s, wp, wf, ow) {
                Im = im;
            }
            public virtual void Draw(SpriteBatch sb) {
                base.Draw(sb);
            }
        }

        public interface IGameWindowComponent
        {
            void Draw(SpriteBatch sb);
            void Update(GameTime gt, MouseState ms, MouseState lms);

            Vector2 GetLocation();
        }

        public delegate void ComponentAction();
    }
