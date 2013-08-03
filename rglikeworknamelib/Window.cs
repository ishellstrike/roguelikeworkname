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

            public Window NewDebugWindow()
            {
                Windows.Add(new Window(new Rectangle(rnd.Next(50, 1600), rnd.Next(50, 1000), 200 + rnd.Next(50, 200), 200 + rnd.Next(50, 200)), "Debug window", true, whitepixel_, font1_, this));
                Windows.Last().Components.Add(new Label(new Vector2(10, 10), "label1", whitepixel_, font1_));
                Windows.Last().Components.Add(new Button(new Vector2(10, 100), "Button1", null, whitepixel_, font1_));
                return Windows.Last();
            }

            public Window NewInfoWindow(string info) {
                int a = (int) Settings.Resolution.X/2 - (int) Settings.Resolution.X/10;
                int b = (int) Settings.Resolution.Y/2 - (int) Settings.Resolution.Y/10;
                
                int w = (int) Settings.Resolution.X/5;
                w = Math.Max(w, (int) font1_.MeasureString(info).X + 20);
                int h = (int) Settings.Resolution.Y/5;
                h = Math.Max(h, (int) font1_.MeasureString(info).Y + 80);

                Windows.Add(new Window(new Rectangle(a, b, w, h), "Info", true,whitepixel_,font1_, this));
                Windows.Last().Components.Add(new Label(new Vector2(10, 10), info,whitepixel_,font1_));
                return Windows.Last();
            }

            //public Window New 


            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.Begin();
                foreach (var component in Windows) {
                    component.Draw(sb, this);
                }
                sb.End();
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                Mopusehook = false;
                Keyboardhook = false;

                if (Windows.Count != 0)
                    for (int i = Windows.Count - 1; i >= 0; i--) {
                        if (Windows.Count > i) {
                            var component = Windows[i];
                            if (component.Readytoclose) {
                                Windows.Remove(component);
                                continue;
                            }
                            component.Update(gt, this, ms, lms);
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
        }

        public class Window : IGameWindowComponent
        {
            internal List<IGameWindowComponent> Components = new List<IGameWindowComponent>();

            private readonly WindowSystem parent_;

            public Rectangle Locate;
            private readonly Color backtransparent_;

            public bool Readytoclose;

            public bool Closable = true;
            public bool Moveable = true;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            public string Name;

            public Window(Rectangle loc, string n, bool clos, Texture2D wp, SpriteFont wf, WindowSystem p) {
                whitepixel_ = wp;
                font1_ = wf;
                parent_ = p;

                Name = n;
                Locate = loc;
                backtransparent_ = Color.Black;
                backtransparent_.A = 220;
                Closable = clos;
                if (Closable)
                    Components.Add(new Button(new Vector2(Locate.Width - 22, -22), "x", Close,whitepixel_,font1_));

                if (Name == "Info") {
                    Components.Add(new Button(new Vector2(Locate.Width/2 - 10, Locate.Height - 50), "OK", Close,whitepixel_,font1_));
                }
            }

            public void Close()
            {
                Readytoclose = true;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, backtransparent_, 0, Vector2.Zero, new Vector2(Locate.Width, Locate.Height), SpriteEffects.None, 0);


                sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(Locate.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y + 20), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(Locate.Width, 2), SpriteEffects.None, 0);

                Vector2 textpos = new Vector2(Locate.X + Locate.Width / 2 - -font1_.MeasureString(Name).X / 2, Locate.Y);

                sb.DrawString(font1_, Name, textpos, Settings.HudСolor);


                sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(2, Locate.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(Locate.Right, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(2, Locate.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Bottom), null, Settings.HudСolor, 0, Vector2.Zero, new Vector2(Locate.Width + 2, 2), SpriteEffects.None, 0);

                for (int i = 0; i < Components.Count; i++) {
                    var component = Components[i];
                    component.Draw(sb, this);
                }
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                if (parent_.Mopusehook == false && lms.X >= Locate.Left && lms.Y >= Locate.Top && lms.X <= Locate.Right && lms.Y <= Locate.Bottom) {
                    parent_.Mopusehook = true;

                    if (lms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {

                        parent_.ToTop(this);
                    }

                    if (Moveable && lms.LeftButton == ButtonState.Pressed && lms.Y <= Locate.Top + 20) {
                        Locate.X += (int)(ms.X - lms.X);
                        Locate.Y += (int)(ms.Y - lms.Y);
                    }

                    for (int i = Components.Count - 1; i >= 0; i--) {
                        var component = Components[i];
                        component.Update(gt, this, ms, lms);
                    }
                }
            }

            public Vector2 GetLocation()
            {
                return new Vector2(Locate.X + 2, Locate.Y + 22);
            }
        }

        public class Label : IGameWindowComponent
        {
            private readonly Vector2 pos_;
            public String Text;
            private Color col_;

            private Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            public Label(Vector2 p, string s, Color c, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                pos_ = p;
                Text = s;
                col_ = c;
            }

            public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                pos_ = p;
                Text = s;
                col_ = Settings.HudСolor;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.DrawString(font1_, Text, owner.GetLocation() + pos_, Settings.HudСolor);
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {

            }

            public Vector2 GetLocation()
            {
                return pos_;
            }
        }

        public class Button : IGameWindowComponent
        {
            private Rectangle locate_;
            public String Text;
            private readonly ComponentAction action;
            private bool aimed_;

            private readonly Texture2D whitepixel_;
            private readonly SpriteFont font1_;

            public Button(Vector2 p, string s, ComponentAction ac, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                locate_.X = (int)p.X;
                locate_.Y = (int)p.Y;
                locate_.Height = 20;
                locate_.Width = (int)(font1_.MeasureString(s).X + 10);
                Text = s;
                action = ac;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                Vector2 realpos = owner.GetLocation() + GetLocation();

                Color col;
                if (!aimed_)
                    col = Settings.HudСolor;
                else col = Color.White;

                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                sb.DrawString(font1_, Text, realpos + new Vector2(5, 0), col);
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                Vector2 realpos = owner.GetLocation() + GetLocation();
                Vector2 realdl = realpos;
                realdl.X += locate_.Width;
                realdl.Y += locate_.Height;

                if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y) {
                    aimed_ = true;
                    if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
                        if (action != null) action();
                } else
                    aimed_ = false;
            }

            public Vector2 GetLocation()
            {
                return new Vector2(locate_.X, locate_.Y);
            }
        }

        public interface IGameWindowComponent
        {
            void Draw(SpriteBatch sb, IGameWindowComponent owner);
            void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms);

            Vector2 GetLocation();
        }

        public delegate void ComponentAction();
    }
