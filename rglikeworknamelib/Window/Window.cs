using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Window : IGameWindowComponent {
        internal List<IGameWindowComponent> Components = new List<IGameWindowComponent>();

        private readonly WindowSystem parent_;

        public Rectangle Locate;
        private readonly Color backtransparent_;

        private Button closeButton_;

        public bool Readytoclose;

        private bool closable_ = true;

        public bool Closable {
            get { return closable_; }
            set {
                closable_ = value;
                if (closeButton_ != null) {
                    closeButton_.Visible = value;
                }
            }
        }

        public bool Moveable = true;
        public bool NoBorder;

        public bool hides;

        public bool Visible { get; set; }

        public object Tag { get; set; }

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
        public Window(Rectangle location, string caption, bool closeable, Texture2D wp, SpriteFont wf,
                      WindowSystem parent) {
            whitepixel_ = wp;
            font1_ = wf;
            parent_ = parent;
            Visible = true;

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
        public Window(Vector2 size, string caption, bool closeable, Texture2D wp, SpriteFont wf, WindowSystem parent) {
            whitepixel_ = wp;
            font1_ = wf;
            parent_ = parent;

            Name = caption;
            Locate = new Rectangle((int) (Settings.Resolution.X/2 - size.X/2),
                                   (int) (Settings.Resolution.Y/2 - size.Y/2), (int) size.X, (int) size.Y);
            backtransparent_ = Color.Black;
            backtransparent_.A = 220;
            Closable = closeable;

            Visible = true;

            closeButton_ = new Button(new Vector2(Locate.Width - 22, -22), "x", whitepixel_, font1_, this);
            closeButton_.onPressed += closeButton__onPressed;
            if (!Closable) {
                closeButton_.Visible = false;
            }

            parent.AddWindow(this);
        }

        public void CenterComponentHor(IGameWindowComponent a) {
            var p = a.GetPosition();
            a.SetPosition(new Vector2((Locate.Width/2 - a.Width/2), p.Y));
        }

        [Obsolete]
        public void CenterComponentVert(IGameWindowComponent a) {
            var p = a.GetPosition();
            a.SetPosition(new Vector2(p.X, (Locate.Height/2 + a.Width)/2));
        }

        private void closeButton__onPressed(object sender, EventArgs e) {
            Close();
        }

        public void AddComponent(IGameWindowComponent component) {
            Components.Add(component);
        }

        public void Close() {
            Visible = false;
        }

        public void Draw(SpriteBatch sb) {
            if (Visible) {
                if (!NoBorder && (!hides || (hides && aimed))) {
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

                for (int i = 0; i < Components.Count; i++) {
                    var component = Components[i];
                    component.Draw(sb);
                }
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms) {
            aimed = false;
            if (Visible) {
                if (parent_.Mopusehook == false && lms.X >= Locate.Left && lms.Y >= Locate.Top &&
                    lms.X <= Locate.Right && lms.Y <= Locate.Bottom) {
                    aimed = true;
                    parent_.Mopusehook = true;

                    if (lms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {

                        parent_.ToTop(this);
                    }

                    if (Moveable && lms.LeftButton == ButtonState.Pressed && lms.Y <= Locate.Top + 20) {
                        Locate.X += (int) (ms.X - lms.X);
                        Locate.Y += (int) (ms.Y - lms.Y);
                    }
                }
                for (int i = Components.Count - 1; i >= 0; i--) {
                    var component = Components[i];
                    component.Update(gt, ms, lms);
                }
            }
        }

        protected bool aimed;

        public Vector2 GetLocation() {
            return new Vector2(Locate.X + 2, Locate.Y + 22);
        }

        public Vector2 GetPosition() {
            return new Vector2(Locate.X, Locate.Y);
        }

        public void SetPosition(Vector2 pos) {
            Locate.X = (int) pos.X;
            Locate.Y = (int) pos.Y;
        }

        public float Width {
            get { return Locate.Width; }
        }

        public float Height {
            get { return Locate.Height; }
        }

        public void OnTop() {
            parent_.ToTop(this);
        }
    }
}
