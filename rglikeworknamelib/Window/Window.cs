using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Window : IGameComponent, IGameContainer {
        private readonly Color backtransparent_;

        private readonly Button closeButton_;
        private readonly WindowSystem parent_;
        internal List<IGameComponent> Components = new List<IGameComponent>();
        public int Id;
        public Rectangle Locate;

        public bool Moveable = true;
        public string Name;
        public bool NoBody;
        public bool NoBorder;
        protected bool aimed;
        private bool closable_ = true;
        private bool hasTextbox_;

        public bool hides;

        /// <summary>
        ///     Create window in exact position whith exact sise
        /// </summary>
        /// <param name="location"></param>
        /// <param name="caption"></param>
        /// <param name="closeable"></param>
        /// <param name="wp"></param>
        /// <param name="wf"></param>
        /// <param name="parent"></param>
        public Window(Rectangle location, string caption, bool closeable, WindowSystem parent) {
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            parent_ = parent;
            Visible = true;

            Name = caption;
            Locate = location;
            backtransparent_ = Color.Black;
            backtransparent_.A = 220;
            Closable = closeable;

            closeButton_ = new Button(new Vector2(Locate.Width - 19, 0), "x", this);
            closeButton_.OnPressed += closeButton__onPressed;
            if (!Closable) {
                closeButton_.Visible = false;
            }

            parent.AddWindow(this);
        }

        /// <summary>
        ///     Create window at center of screen
        /// </summary>
        /// <param name="size"></param>
        /// <param name="caption"></param>
        /// <param name="closeable"></param>
        /// <param name="wp"></param>
        /// <param name="wf"></param>
        /// <param name="parent"></param>
        public Window(Vector2 size, string caption, bool closeable, WindowSystem parent) {
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            parent_ = parent;

            Name = caption;
            Locate = new Rectangle((int) (Settings.Resolution.X/2 - size.X/2),
                                   (int) (Settings.Resolution.Y/2 - size.Y/2), (int) size.X, (int) size.Y);
            backtransparent_ = Color.Black;
            backtransparent_.A = 220;
            Closable = closeable;

            Visible = true;

            closeButton_ = new Button(new Vector2(Locate.Width - 19, -20), "x", this);
            closeButton_.OnPressed += closeButton__onPressed;
            if (!Closable) {
                closeButton_.Visible = false;
            }

            parent.AddWindow(this);
        }

        public bool Closable {
            get { return closable_; }
            set {
                closable_ = value;
                if (closeButton_ != null) {
                    closeButton_.Visible = value;
                }
            }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb) {
            if (Visible) {
                if (!NoBody) {
                    sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y + 20), null, backtransparent_, 0, Vector2.Zero,
                            new Vector2(Locate.Width, Locate.Height - 20), SpriteEffects.None, 0);
                }

                if (!NoBorder && (!hides || (hides && aimed))) {
                    sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, backtransparent_, 0, Vector2.Zero,
                            new Vector2(Locate.Width, 20), SpriteEffects.None, 0);

                    sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y), null, Settings.HudСolor, 0, Vector2.Zero,
                            new Vector2(Locate.Width, 2), SpriteEffects.None, 0);
                    sb.Draw(whitepixel_, new Vector2(Locate.X, Locate.Y + 20), null, Settings.HudСolor, 0,
                            Vector2.Zero,
                            new Vector2(Locate.Width, 2), SpriteEffects.None, 0);

                    var textpos = new Vector2(Locate.X + Locate.Width/2 - 11 - font1_.MeasureString(Name).X/2,
                                              Locate.Y);

                    string compose = Name;

                    if (Settings.DebugInfo) {
                        compose = Locate.ToString() + Environment.NewLine + compose;
                    }

                    sb.DrawString(font1_, compose, textpos, Settings.HudСolor);


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
                    IGameComponent component = Components[i];
                    component.Draw(sb);
                }
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            aimed = false;
            if (Visible && !mh) {
                if (hasTextbox_) {
                    parent_.Keyboardhook = true;
                }
                if (!parent_.Mopusehook && lms.X >= Locate.Left && lms.Y >= Locate.Top &&
                    lms.X <= Locate.Right && lms.Y <= Locate.Bottom) {
                    mh = true;
                    parent_.Mopusehook = true;
                    aimed = true;

                    if (lms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) {
                        parent_.ToTop(this);
                    }

                    if (Moveable && lms.LeftButton == ButtonState.Pressed && lms.Y <= Locate.Top + 20) {
                        Locate.X += (ms.X - lms.X);
                        Locate.Y += (ms.Y - lms.Y);

                        if (Locate.X < 0) {
                            Locate.X = 0;
                        }
                        if (Locate.Y < 0) {
                            Locate.Y = 0;
                        }
                        if (Locate.X > Settings.Resolution.X - 20) {
                            Locate.X = (int) Settings.Resolution.X - 20;
                        }
                        if (Locate.Y > Settings.Resolution.Y - 20) {
                            Locate.X = (int) Settings.Resolution.Y - 20;
                        }
                    }
                }
                for (int i = Components.Count - 1; i >= 0; i--) {
                    IGameComponent component = Components[i];
                    component.Update(gt, ms, lms, ks, lks, mh);
                }
            }
        }

        public Vector2 GetPosition() {
            return new Vector2(Locate.X, Locate.Y + 20);
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

        public Texture2D whitepixel_ { get; set; }
        public SpriteFont font1_ { get; set; }

        public void CenterComponentHor(IGameComponent component) {
            Vector2 p = component.GetPosition();
            component.SetPosition(new Vector2((Locate.Width/2 - component.Width/2), p.Y));
        }

        [Obsolete]
        public void CenterComponentVert(IGameComponent component) {
            Vector2 p = component.GetPosition();
            component.SetPosition(new Vector2(p.X, (Locate.Height/2 + component.Width)/2));
        }

        public void AddComponent(IGameComponent component) {
            Components.Add(component);
            if (component is TextBox) {
                hasTextbox_ = true;
            }
        }

        public bool MouseClickCatched { get; set; }

        private void closeButton__onPressed(object sender, EventArgs e) {
            Close();
        }

        public void Close() {
            Visible = false;
        }

        public Vector2 GetLocation() {
            return new Vector2(Locate.X + 2, Locate.Y + 22);
        }

        public void OnTop() {
            parent_.ToTop(this);
        }
    }
}