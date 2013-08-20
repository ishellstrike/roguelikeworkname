using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class ListContainer : IGameComponent, IGameContainer {
        private Rectangle location_;
        public List<IGameComponent> Items = new List<IGameComponent>();
        private List<IGameComponent> Components = new List<IGameComponent>(); 
        public int fromI_;
        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;
        private IGameContainer parent_;
        public bool Visible { get; set; }

        public object Tag { get; set; }

        private Button buttonUp_, buttonDown_;
        private VerticalProgressBar progress_;

        public ListContainer(Rectangle loc, Texture2D wp, SpriteFont sf, IGameContainer win)
        {
            location_ = loc;
            whitepixel_ = wp;
            font1_ = sf;
            parent_ = win;
            parent_.AddComponent(this);

            Visible = true;



            buttonUp_ = new Button(Vector2.Zero, "^", wp, sf, this);
            Vector2 bup = new Vector2(Width - buttonUp_.Width, 0);
            buttonUp_.SetPosition(bup);
            buttonUp_.onPressed += buttonUp__onPressed;

            buttonDown_ = new Button(Vector2.Zero, "v", wp, sf, this);
            Vector2 bdow = new Vector2(Width - buttonDown_.Width, location_.Height - buttonDown_.Height);
            buttonDown_.SetPosition(bdow);
            buttonDown_.onPressed += buttonDown__onPressed;

            progress_ = new VerticalProgressBar(new Rectangle((int)bup.X, (int)(bup.Y + buttonUp_.Height), (int)buttonUp_.Width, (int)(bdow.Y - bup.Y)), "", wp, font1_, this);
        
            RecalcContainer();
        }

        void buttonDown__onPressed(object sender, EventArgs e)
        {
            ListDown();

            RecalcContainer();
        }

        void buttonUp__onPressed(object sender, EventArgs e)
        {
            ListUp();

            RecalcContainer();
        }

        private void RecalcContainer() {
            progress_.Max = Items.Count;
            progress_.Progress = fromI_ + ToShow();

            var l = Vector2.Zero;

            foreach (var item in Items)
            {
                item.Visible = false;
            }

            float curBottom = 0;
            int fromNow = fromI_;
            if (fromNow < Items.Count)
            {
                while (true)
                {
                    if (curBottom + Items[fromNow].Height + 10 > location_.Bottom)
                    {
                        break;
                    }
                    Items[fromNow].Visible = true;
                    Items[fromNow].SetPosition(new Vector2(l.X + 10, curBottom));

                    curBottom += Items[fromNow].Height + 10;
                    fromNow++;
                    if (fromNow > Items.Count - 1)
                    {
                        break;
                    }
                }
            }
        }

        public List<IGameComponent> GetItems() {
            return Items;
        }

        public void AddItem(IGameComponent it) {
            Items.Add(it);

            RecalcContainer();
        }
             
        public void RemoveItem(IGameComponent it) {
            if(Items.Contains(it)) {
                Items.Remove(it);
            }

            RecalcContainer();
        }

        public void Clear() {
            fromI_ = 0;

            Items.Clear();
            Components.Clear();

            Components.Add(buttonUp_);
            Components.Add(buttonDown_);
            Components.Add(progress_);

            RecalcContainer();
        }

        public void Draw(SpriteBatch sb) {

            if (Visible) {
                var a = GetPosition();
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

                foreach (var item in Components) {
                    item.Draw(sb);
                }
                     
            }
        }

        public int ToShow()
        {
            int toshow = location_.Height / 30;
            return toshow > Items.Count - 1 ? Items.Count - 1 : toshow;
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, bool h) {
            if (Visible) {
                foreach (var item in Components) {
                    item.Update(gt, ms, lms, h);
                }
            }
        }

        public void ListDown() {
            fromI_ ++;
            var a = ToShow();
            if (fromI_ + a > Items.Count - 1) fromI_ = Items.Count - 1 - a;

            RecalcContainer();
        }

        public void ListUp() {
            fromI_--;
            if (fromI_ < 0) fromI_ = 0;

            RecalcContainer();
        }

        public Vector2 GetPosition() {
            return new Vector2(location_.X, location_.Y) + parent_.GetPosition();
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

            RecalcContainer();
        }

        public void CenterComponentHor(IGameComponent a) {
           
        }

        public void CenterComponentVert(IGameComponent a) {
           
        }

        public void AddComponent(IGameComponent component) {
            Components.Add(component);
        }

        public bool MouseClickCatched { get; set; }
    }
}