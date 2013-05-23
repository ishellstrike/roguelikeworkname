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
            private static List<Window> windows = new List<Window>();
            Random rnd = new Random();
            public bool mopusehook, keyboardhook;

            private Texture2D whitepixel_;
            private SpriteFont font1_;

            public WindowSystem(Texture2D wp, SpriteFont f1) {
                whitepixel_ = wp;
                font1_ = f1;
            }

            public Window NewDebugWindow()
            {
                windows.Add(new Window(new Rectangle(rnd.Next(50, 1600), rnd.Next(50, 1000), 200 + rnd.Next(50, 200), 200 + rnd.Next(50, 200)), "Debug window", true, whitepixel_, font1_, this));
                windows.Last().components.Add(new Label(new Vector2(10, 10), "label1", whitepixel_, font1_));
                windows.Last().components.Add(new Button(new Vector2(10, 100), "Button1", null, whitepixel_, font1_));
                return windows.Last();
            }

            public Window NewInfoWindow(string info) {
                int a = (int) Settings.Resolution.X/2 - (int) Settings.Resolution.X/10;
                int b = (int) Settings.Resolution.Y/2 - (int) Settings.Resolution.Y/10;
                
                int w = (int) Settings.Resolution.X/5;
                w = Math.Max(w, (int) font1_.MeasureString(info).X + 20);
                int h = (int) Settings.Resolution.Y/5;
                h = Math.Max(h, (int) font1_.MeasureString(info).Y + 80);

                windows.Add(new Window(new Rectangle(a, b, w, h), "Info", true,whitepixel_,font1_, this));
                windows.Last().components.Add(new Label(new Vector2(10, 10), info,whitepixel_,font1_));
                return windows.Last();
            }

            //public Window New 


            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.Begin();
                foreach (var component in windows) {
                    component.Draw(sb, this);
                }
                sb.End();
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                mopusehook = false;
                keyboardhook = false;

                if (windows.Count != 0)
                    for (int i = windows.Count - 1; i >= 0; i--) {
                        if (windows.Count > i) {
                            var component = windows[i];
                            if (component.readytoclose) {
                                windows.Remove(component);
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
                windows.Remove(win);
                windows.Insert(windows.Count, win);
            }
        }

        public class Window : IGameWindowComponent
        {
            internal List<IGameWindowComponent> components = new List<IGameWindowComponent>();

            private WindowSystem parent;

            public Rectangle locate;
            private Color backtransparent;

            public bool readytoclose = false;

            public bool closable = true;
            public bool moveable = true;

            private Texture2D whitepixel_;
            private SpriteFont font1_;

            public string name;

            public Window(Rectangle loc, string n, bool clos, Texture2D wp, SpriteFont wf, WindowSystem p) {
                whitepixel_ = wp;
                font1_ = wf;
                parent = p;

                name = n;
                locate = loc;
                backtransparent = Color.Black;
                backtransparent.A = 220;
                closable = clos;
                if (closable)
                    components.Add(new Button(new Vector2(locate.Width - 22, -22), "x", Close,whitepixel_,font1_));

                if (name == "Info") {
                    components.Add(new Button(new Vector2(locate.Width/2 - 10, locate.Height - 50), "OK", Close,whitepixel_,font1_));
                }
            }

            public void Close()
            {
                readytoclose = true;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y), null, backtransparent, 0, Vector2.Zero, new Vector2(locate.Width, locate.Height), SpriteEffects.None, 0);


                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y), null, Settings.HUDcolor, 0, Vector2.Zero, new Vector2(locate.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y + 20), null, Settings.HUDcolor, 0, Vector2.Zero, new Vector2(locate.Width, 2), SpriteEffects.None, 0);

                Vector2 textpos = new Vector2(locate.X + locate.Width / 2 - -font1_.MeasureString(name).X / 2, locate.Y);

                sb.DrawString(font1_, name, textpos, Settings.HUDcolor);


                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y), null, Settings.HUDcolor, 0, Vector2.Zero, new Vector2(2, locate.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.Right, locate.Y), null, Settings.HUDcolor, 0, Vector2.Zero, new Vector2(2, locate.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Bottom), null, Settings.HUDcolor, 0, Vector2.Zero, new Vector2(locate.Width + 2, 2), SpriteEffects.None, 0);

                for (int i = 0; i < components.Count; i++) {
                    var component = components[i];
                    component.Draw(sb, this);
                }
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                if (parent.mopusehook == false && lms.X >= locate.Left && lms.Y >= locate.Top && lms.X <= locate.Right && lms.Y <= locate.Bottom) {
                    parent.mopusehook = true;

                    if (lms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {

                        parent.ToTop(this);
                    }

                    if (moveable && lms.LeftButton == ButtonState.Pressed && lms.Y <= locate.Top + 20) {
                        locate.X += (int)(ms.X - lms.X);
                        locate.Y += (int)(ms.Y - lms.Y);
                    }

                    for (int i = components.Count - 1; i >= 0; i--) {
                        var component = components[i];
                        component.Update(gt, this, ms, lms);
                    }
                }
            }

            public Vector2 GetLocation()
            {
                return new Vector2(locate.X + 2, locate.Y + 22);
            }
        }

        public class Label : IGameWindowComponent
        {
            private Vector2 pos;
            public String text;
            private Color col;

            private Texture2D whitepixel_;
            private SpriteFont font1_;

            public Label(Vector2 p, string s, Color c, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                pos = p;
                text = s;
                col = c;
            }

            public Label(Vector2 p, string s, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                pos = p;
                text = s;
                col = Settings.HUDcolor;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                sb.DrawString(font1_, text, owner.GetLocation() + pos, Settings.HUDcolor);
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {

            }

            public Vector2 GetLocation()
            {
                return pos;
            }
        }

        public class Button : IGameWindowComponent
        {
            private Rectangle locate;
            public String text;
            private ComponentAction action;
            private bool aimed = false;

            private Texture2D whitepixel_;
            private SpriteFont font1_;

            public Button(Vector2 p, string s, ComponentAction ac, Texture2D wp, SpriteFont wf)
            {
                whitepixel_ = wp;
                font1_ = wf;
                locate.X = (int)p.X;
                locate.Y = (int)p.Y;
                locate.Height = 20;
                locate.Width = (int)(font1_.MeasureString(s).X + 10);
                text = s;
                action = ac;
            }

            public void Draw(SpriteBatch sb, IGameWindowComponent owner)
            {
                Vector2 realpos = owner.GetLocation() + GetLocation();

                Color col;
                if (!aimed)
                    col = Settings.HUDcolor;
                else col = Color.White;

                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(locate.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(2, locate.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.Right, locate.Y) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(2, locate.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate.X, locate.Bottom) + owner.GetLocation(), null, col, 0, Vector2.Zero, new Vector2(locate.Width + 2, 2), SpriteEffects.None, 0);

                sb.DrawString(font1_, text, realpos + new Vector2(5, 0), col);
            }

            public void Update(GameTime gt, IGameWindowComponent owner, MouseState ms, MouseState lms)
            {
                Vector2 realpos = owner.GetLocation() + GetLocation();
                Vector2 realdl = realpos;
                realdl.X += locate.Width;
                realdl.Y += locate.Height;

                if (ms.X >= realpos.X && ms.Y >= realpos.Y && ms.X <= realdl.X && ms.Y <= realdl.Y) {
                    aimed = true;
                    if (lms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed)
                        if (action != null) action();
                } else
                    aimed = false;
            }

            public Vector2 GetLocation()
            {
                return new Vector2(locate.X, locate.Y);
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
