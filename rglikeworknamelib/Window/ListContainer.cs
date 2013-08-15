using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ListContainer : IGameWindowComponent {
        private Rectangle location_;
        public List<IGameWindowComponent> Items = new List<IGameWindowComponent>();
        public int fromI_;
        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;
        private Window parent_;
        public bool Visible { get; set; }

        public object Tag { get; set; }

        private Button buttonUp_, buttonDown_;
        private VerticalProgressBar progress_;

        public ListContainer(Rectangle loc, Texture2D wp, SpriteFont sf, Window win) {
            location_ = loc;
            whitepixel_ = wp;
            font1_ = sf;
            parent_ = win;
            parent_.AddComponent(this);

            Visible = true;

            buttonUp_ = new Button(new Vector2(location_.Width - 30 + loc.X, 2 + loc.Y - 20), "^", wp, sf, win);
            buttonUp_.onPressed += buttonUp__onPressed;
            buttonDown_ = new Button(new Vector2(location_.Width - 30 + loc.X, location_.Height - 25 + loc.Y - 20), "v", wp, sf, win);
            buttonDown_.onPressed += buttonDown__onPressed;
            progress_ = new VerticalProgressBar(new Rectangle((int)buttonUp_.GetPosition().X, (int)buttonUp_.GetPosition().Y + 34, 19, (int)buttonDown_.GetPosition().Y - (int)buttonUp_.GetPosition().Y - 50), "", wp, font1_, win);
        }

        void buttonDown__onPressed(object sender, EventArgs e)
        {
            ListDown();

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }

        void buttonUp__onPressed(object sender, EventArgs e)
        {
            ListUp();

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }

        public List<IGameWindowComponent> GetItems() {
            return Items;
        }

        public void AddItem(IGameWindowComponent it) {
            Items.Add(it);

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }
             
        public void RemoveItem(IGameWindowComponent it) {
            if(Items.Contains(it)) {
                Items.Remove(it);
            }

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }

        public void Clear() {
            fromI_ = 0;
            foreach (var item in Items) {
                parent_.Components.Remove(item);
            }
            Items.Clear();

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }

        public void Draw(SpriteBatch sb) {
            var ts = ToShow();

            var l = GetPosition();

            foreach (var item in Items) {
                item.Visible = false;
            }

            float curBottom = 0;
            int fromNow = fromI_;
            if (fromNow < Items.Count) {
                while (true) {
                    if (curBottom + Items[fromNow].Height + 10 > location_.Bottom) {
                        break;
                    }
                    Items[fromNow].Visible = true;
                    Items[fromNow].SetPosition(new Vector2(l.X + 10, curBottom));

                    curBottom += Items[fromNow].Height + 10;
                    fromNow++;
                    if (fromNow > Items.Count - 1) {
                        break;
                    } 
                }
            }

            if (Visible) {
                var a = GetPosition() + parent_.GetPosition();
                var b = new Rectangle((int) a.X, (int) a.Y, location_.Width, location_.Height);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudÑolor, 0, Vector2.Zero,
                        new Vector2(b.Width, 2), SpriteEffects.None, 0);

                sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudÑolor, 0, Vector2.Zero,
                        new Vector2(2, b.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.Right, b.Y), null, Settings.HudÑolor, 0,
                        Vector2.Zero,
                        new Vector2(2, b.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(b.X, b.Bottom), null, Settings.HudÑolor, 0,
                        Vector2.Zero,
                        new Vector2(b.Width + 2, 2), SpriteEffects.None, 0);
                     
            }
        }

        public int ToShow()
        {
            int toshow = location_.Height / 30;
            return toshow = toshow > Items.Count - 1 ? Items.Count - 1 : toshow;
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms) {
                
        }

        public void ListDown() {
            fromI_ ++;
            var a = ToShow();
            if (fromI_ + a > Items.Count - 1) fromI_ = Items.Count - 1 - a;
        }

        public void ListUp() {
            fromI_--;
            if (fromI_ < 0) fromI_ = 0;
        }

        public Vector2 GetPosition() {
            return new Vector2(location_.X, location_.Y);
        }

        public void SetPosition(Vector2 pos) {
            location_.X = (int)pos.X;
            location_.Y = (int)pos.Y;
        }

        public float Width
        {
            get { return location_.Width; }
        }

        public float Height
        {
            get { return location_.Height; }
        }

        public void SetVisible(bool vis) {
                
        }

        public int GetTag()
        {
            return 0;
        }

        public void ScrollBottom() {
            var a = ToShow();
            fromI_ = Items.Count - a;
            if (fromI_ + a > Items.Count - 1) fromI_ = Items.Count - 1 - a;

            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();
        }
    }
}