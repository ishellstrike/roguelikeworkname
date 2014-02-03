using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window
{
    public class ListBoxItem {
        public string Text;
        public object Tag;
        public event EventHandler<ListBoxItemPressEventArgs> OnMousePressed;

        public void OnPress(MouseState ms, MouseState lms)
        {
            if (OnMousePressed != null) {
                OnMousePressed(this, new ListBoxItemPressEventArgs{Ms = ms, Lms = lms});
            }
        }

        public static implicit operator ListBoxItem(string s)
        {
         return new ListBoxItem{Text = s};   
        }
    }

    public class ListBoxItemPressEventArgs : EventArgs {
        public MouseState Ms, Lms;
    }

    public class InteractiveListBox : IGameComponent {
        private Rectangle location_;
        public float Width { get; private set; }
        public float Height { get; private set; }
        public bool Visible { get; set; }
        public object Tag { get; set; }
        public IGameContainer Parent;
        private Texture2D whitepixel_;
        private SpriteFont font1_;
        public List<ListBoxItem> Items = new List<ListBoxItem>();
        private Button buttonUp_;
        private Button buttonDown_;
        private VerticalSlider progress_;
        private int topIndex;
        private int SelectedIndex;
        private int textH, textW;
        private int aimed;

        public InteractiveListBox(Rectangle loc, IGameContainer parent) {
            location_ = loc;
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            Parent = parent;
            Parent.AddComponent(this);

            textH = (int) font1_.MeasureString("O").Y;
            textW = (int)font1_.MeasureString("O").X;

            var ptop = new Vector2(loc.X + 1, loc.Y + 19);
            var p = new Vector2(loc.Width + 1, loc.Height + 19);

            buttonUp_ = new Button(Vector2.Zero, "^", Parent);
            var bup = new Vector2(p.X, ptop.Y - buttonUp_.Width);
            buttonUp_.SetPosition(bup);
            buttonUp_.OnPressed += buttonUp__onPressed;

            buttonDown_ = new Button(Vector2.Zero, "v", Parent);
            var bdow = new Vector2(p.X, p.Y - buttonUp_.Width);
            buttonDown_.SetPosition(bdow);
            buttonDown_.OnPressed += buttonDown__onPressed;

            progress_ =
                new VerticalSlider(
                    new Rectangle((int)(bup.X), (int)(bup.Y + buttonUp_.Height), (int)buttonUp_.Width,
                                  (int)(bdow.Y - bup.Y)), parent);

            Visible = true;
        }

        private void buttonDown__onPressed(object sender, EventArgs e) {
            topIndex++;
            if (topIndex > lastDrawed) {
                topIndex--;
            }
        }

        private void buttonUp__onPressed(object sender, EventArgs e) {
            topIndex--;
            if (topIndex < 0) {
                topIndex = 0;
            }
        }

        public int lastDrawed;
        public void Draw(SpriteBatch sb) {
            Vector2 a = GetPosition();
            var b = new Rectangle((int)a.X, (int)a.Y, location_.Width, location_.Height);
            DrawBox(sb, b, 2);

            int bottom = 3;
            int item = topIndex;
            var p = GetPosition();

            while (bottom < location_.Height - textH - 6) {
                if (item >= Items.Count) {
                    break;
                }
                if (aimed == item) {
                    var tp = p + new Vector2(10, bottom);
                    var txt = Items[item].Text;
                    var c = new Rectangle((int)tp.X - 1, (int)tp.Y - 1, txt.Length*textW + 2, textH + 2);
                    DrawBox(sb, c, 1);
                    sb.DrawString(font1_, txt, tp, Color.White);
                }
                else {
                    sb.DrawString(font1_, Items[item].Text, p + new Vector2(10, bottom), Color.White);
                }
                
                item++;
                bottom += textH + 2;
            }

            lastDrawed = item - topIndex;
            progress_.Start = topIndex;
            progress_.Size = item - topIndex;
            progress_.Max = Items.Count;
        }

        private void DrawBox(SpriteBatch sb, Rectangle b, float lineW) {
            sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudСolor, 0, Vector2.Zero,
                new Vector2(b.Width, lineW), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(b.X, b.Y), null, Settings.HudСolor, 0, Vector2.Zero,
                new Vector2(lineW, b.Height), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(b.Right, b.Y), null, Settings.HudСolor, 0,
                Vector2.Zero,
                new Vector2(lineW, b.Height + lineW), SpriteEffects.None, 0);
            sb.Draw(whitepixel_, new Vector2(b.X, b.Bottom), null, Settings.HudСolor, 0,
                Vector2.Zero,
                new Vector2(b.Width + lineW, lineW), SpriteEffects.None, 0);
        }

        bool hooked;
        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            var p = GetPosition();
            var ps = p + new Vector2(location_.Width, location_.Height);
            if (ms.X > p.X && ms.Y > p.Y && ms.X < ps.X && ms.Y < ps.Y) {
                aimed = (int) ((ms.Y - p.Y)/(textH + 2)) + topIndex;
                if (aimed >= Items.Count) {
                    aimed = -1;
                   
                } else if (!(ms.X > p.X + 10) || !(ms.X < p.X + 10 + Items[aimed].Text.Length*textW)) {
                    aimed = -1;
                }

                if (aimed != -1 && (ms.RightButton == ButtonState.Pressed || ms.LeftButton == ButtonState.Pressed)) {
                    Items[aimed].OnPress(ms, lms);
                }
            }
            else {
                aimed = -1;
            }

            var pos = progress_.GetPosition();
            
            if (ms.X >= pos.X && ms.Y >= pos.Y && ms.Y <= pos.X + progress_.Width && ms.Y <= pos.Y + progress_.Height)
            {
                if (ms.LeftButton == ButtonState.Pressed && lms.LeftButton == ButtonState.Released)
                {
                    hooked = true;
                }
            }
            if (ms.LeftButton == ButtonState.Released)
            {
                hooked = false;
            }

            if (hooked)
            {
                var cent = progress_.Max * (ms.Y - pos.Y) / progress_.Height;
                progress_.Start = (int)(cent) - progress_.Size / 2;
                if (progress_.Start < 0)
                {
                    progress_.Start = 0;
                }

                if (progress_.Start + progress_.Size > progress_.Max)
                {
                    progress_.Start = progress_.Max - progress_.Size;
                }

                topIndex = progress_.Start;
            }
        }

        public Vector2 GetPosition() {
            return new Vector2(location_.X, location_.Y) + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            location_ = new Rectangle((int) pos.X, (int) pos.Y, location_.Width, location_.Height);
        }
    }
}
