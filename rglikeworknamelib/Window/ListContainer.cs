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
        public int FromI;
        public Texture2D whitepixel_ { get;set; }
        public SpriteFont font1_ { get;set; }
        private IGameContainer parent_;
        public bool Visible { get; set; }

        public object Tag { get; set; }

        private Button buttonUp_, buttonDown_;
        private VerticalProgressBar progress_;

        public ListContainer(Rectangle loc, IGameContainer parent)
        {
            location_ = loc;
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            parent_ = parent;
            parent_.AddComponent(this);

            Visible = true;



            buttonUp_ = new Button(Vector2.Zero, "^", this);
            Vector2 bup = new Vector2(Width - buttonUp_.Width, 0);
            buttonUp_.SetPosition(bup);
            buttonUp_.OnPressed += buttonUp__onPressed;

            buttonDown_ = new Button(Vector2.Zero, "v", this);
            Vector2 bdow = new Vector2(Width - buttonDown_.Width, location_.Height - buttonDown_.Height);
            buttonDown_.SetPosition(bdow);
            buttonDown_.OnPressed += buttonDown__onPressed;

            progress_ = new VerticalProgressBar(new Rectangle((int)bup.X, (int)(bup.Y + buttonUp_.Height), (int)buttonUp_.Width, (int)(bdow.Y - bup.Y)), "", this);
        
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

            var l = Vector2.Zero;

            foreach (var item in Items)
            {
                item.Visible = false;
            }

            float curBottom = 0;
            int count = 0;
            int fromNow = FromI;
            if (fromNow < Items.Count)
            {
                while (true)
                {
                    if (curBottom + Items[fromNow].Height + 10 > location_.Bottom - location_.Top)
                    {
                        break;
                    }
                    Items[fromNow].Visible = true;
                    Items[fromNow].SetPosition(new Vector2(l.X + 10, curBottom));
                    count++;

                    curBottom += Items[fromNow].Height + 10;
                    fromNow++;
                    if (fromNow > Items.Count - 1)
                    {
                        break;
                    }
                }
            }

            progress_.Progress = FromI + count;
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
            FromI = 0;

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

                foreach (var item in Components) {
                    item.Draw(sb);
                }
                     
            }
        }

        public int ToShow()
        {
            float cur = location_.Top;
            int lastN = 0;
            for (int i = FromI; i < Items.Count; i++)
            {
                cur += Items[i].Height + 10;
                if (cur > location_.Bottom)
                {
                    break;
                }
                lastN = i;
            }

            return lastN - FromI;
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            if (Visible) {
                for (int i = 0; i < Components.Count; i++) {
                    Components[i].Update(gt, ms, lms, ks, lks, mh);
                }
            }
        }

        public void ListDown() {
            FromI ++;

            float cur = location_.Bottom;
            int lastN = 0;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                cur -= Items[i].Height + 10;
                if (cur < location_.Top)
                {
                    break;
                }
                lastN = i;
            }
            if (FromI > lastN) FromI = lastN;

            RecalcContainer();
        }

        public void ListUp() {
            FromI--;

            if (FromI < 0) FromI = 0;

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
            float cur = location_.Bottom;
            int lastN = 0;
            for (int i = Items.Count - 1; i >= 0; i--) {
                cur -= Items[i].Height+10;
                if(cur < location_.Top) {
                    break;
                }
                lastN = i;
            }
            FromI = lastN;

            RecalcContainer();
        }

        public void CenterComponentHor(IGameComponent component) {
           
        }

        public void CenterComponentVert(IGameComponent component) {
           
        }

        public void AddComponent(IGameComponent component) {
            Components.Add(component);
        }

        public bool MouseClickCatched { get; set; }
    }
}