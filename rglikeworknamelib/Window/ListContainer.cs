using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ListContainer : IGameComponent, IGameContainer {
        private readonly List<IGameComponent> Components = new List<IGameComponent>();
        private readonly Button buttonDown_;
        private readonly Button buttonUp_;
        private readonly IGameContainer parent_;
        private readonly VerticalProgressBar progress_;

        private readonly bool ready;
        public int FromI;
        private Rectangle location_;

        public ListContainer(Rectangle loc, IGameContainer parent) {
            location_ = loc;
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            parent_ = parent;
            parent_.AddComponent(this);

            Visible = true;


            buttonUp_ = new Button(Vector2.Zero, "^", this);
            var bup = new Vector2(Width - buttonUp_.Width, 0);
            buttonUp_.SetPosition(bup);
            buttonUp_.OnPressed += buttonUp__onPressed;

            buttonDown_ = new Button(Vector2.Zero, "v", this);
            var bdow = new Vector2(Width - buttonDown_.Width, location_.Height - buttonDown_.Height);
            buttonDown_.SetPosition(bdow);
            buttonDown_.OnPressed += buttonDown__onPressed;

            progress_ =
                new VerticalProgressBar(
                    new Rectangle((int) bup.X, (int) (bup.Y + buttonUp_.Height), (int) buttonUp_.Width,
                                  (int) (bdow.Y - bup.Y)), "", this);

            ready = true;
            RecalcContainer();
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public void Draw(SpriteBatch sb) {
            if (Visible) {
                Vector2 a = GetPosition();
                var b = new Rectangle((int) a.X, (int) a.Y, location_.Width, location_.Height);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.Hud—olor, 0, Vector2.Zero,
                        new Vector2(b.Width, 2), SpriteEffects.None, 0);

                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.Hud—olor, 0, Vector2.Zero,
                        new Vector2(2, b.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.Right, b.Y), null, Settings.Hud—olor, 0,
                        Vector2.Zero,
                        new Vector2(2, b.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Bottom), null, Settings.Hud—olor, 0,
                        Vector2.Zero,
                        new Vector2(b.Width + 2, 2), SpriteEffects.None, 0);

                for (int i = 0; i < Components.Count; i++) {
                    Components[i].Draw(sb);
                }
            }
        }

        private int scVal = 0;
        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            if (Visible) {
                for (int i = 0; i < Components.Count; i++) {
                    Components[i].Update(gt, ms, lms, ks, lks, mh);
                }
                var a = GetPosition();
                if (ms.X >= a.X && ms.Y >= a.Y && ms.X <= a.X + Width && ms.Y <= a.Y + Height && mh) {
                    if (scVal > ms.ScrollWheelValue) {
                        buttonDown__onPressed(null, null);
                    } else if (scVal < ms.ScrollWheelValue) {
                        buttonUp__onPressed(null, null);
                    }
                }
                scVal = ms.ScrollWheelValue;
            }
        }

        public Vector2 GetPosition() {
            return new Vector2(location_.X, location_.Y) + parent_.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            location_.X = (int) pos.X;
            location_.Y = (int) pos.Y;
        }

        public float Width {
            get { return location_.Width; }
        }

        public float Height {
            get { return location_.Height; }
        }

        public Texture2D whitepixel_ { get; set; }
        public SpriteFont font1_ { get; set; }

        public void CenterComponentHor(IGameComponent component) {
        }

        public void CenterComponentVert(IGameComponent component) {
        }

        public void AddComponent(IGameComponent component) {
            Components.Add(component);
            RecalcContainer();
        }

        public bool MouseClickCatched { get; set; }

        private void buttonDown__onPressed(object sender, EventArgs e) {
            ListDown();

            RecalcContainer();
        }

        private void buttonUp__onPressed(object sender, EventArgs e) {
            ListUp();

            RecalcContainer();
        }

        private void RecalcContainer() {
            if (ready) {
                progress_.Max = Components.Count;

                Vector2 l = Vector2.Zero;

                for (int i = 0; i < Components.Count; i++) {
                    IGameComponent item = Components[i];
                    if (item == buttonUp_ || item == buttonDown_ || item == progress_) {
                        continue;
                    }
                    item.Visible = false;
                }

                float curBottom = 0;
                int count = 0;
                int fromNow = FromI + 3;
                if (fromNow < Components.Count) {
                    for (;;) {
                        if (curBottom + Components[fromNow].Height + 10 > location_.Bottom - location_.Top) {
                            break;
                        }
                        Components[fromNow].Visible = true;
                        Components[fromNow].SetPosition(new Vector2(l.X + 10, curBottom));
                        count++;

                        curBottom += Components[fromNow].Height + 2;
                        fromNow++;
                        if (fromNow > Components.Count - 1) {
                            break;
                        }
                    }
                }

                progress_.Progress = FromI + count;
            }
        }

        public List<IGameComponent> GetItems() {
            return Components;
        }

        public void RemoveItem(IGameComponent it) {
            if (Components.Contains(it)) {
                Components.Remove(it);
            }

            RecalcContainer();
        }

        public void Clear() {
            FromI = 0;

            Components.Clear();

            Components.Add(buttonUp_);
            Components.Add(buttonDown_);
            Components.Add(progress_);

            RecalcContainer();
        }

        public int ToShow() {
            float cur = location_.Top;
            int lastN = 0;
            for (int i = FromI; i < Components.Count; i++) {
                cur += Components[i].Height + 2;
                if (cur > location_.Bottom) {
                    break;
                }
                lastN = i;
            }

            return lastN - FromI;
        }

        public void ListDown() {
            FromI ++;

            float cur = location_.Bottom;
            int lastN = 0;
            for (int i = Components.Count - 1; i >= 0; i--) {
                cur -= Components[i].Height + 2;
                if (cur < location_.Top) {
                    break;
                }
                lastN = i;
            }
            if (FromI > lastN) {
                FromI = lastN;
            }

            RecalcContainer();
        }

        public void ListUp() {
            FromI--;

            if (FromI < 0) {
                FromI = 0;
            }

            RecalcContainer();
        }

        public void SetVisible(bool vis) {
        }

        public int GetTag() {
            return 0;
        }

        public void ScrollBottom() {
            float cur = location_.Bottom;
            int lastN = 0;
            for (int i = Components.Count - 1; i >= 0; i--) {
                cur -= Components[i].Height + 2;
                if (cur < location_.Top) {
                    break;
                }
                lastN = i;
            }
            FromI = lastN - 3;

            RecalcContainer();
        }
    }
}