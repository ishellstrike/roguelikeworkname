using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Window
{
    public class ListBoxItem {
        public string Text;
        public object Tag;
        public double Progress;
        public event EventHandler<ListBoxItemPressEventArgs> OnMousePressed;
        public Color Color = Color.White;

        public ListBoxItem(string message, Color col) {
            Text = message;
            Color = col;
        }

        public ListBoxItem(string message, Color col, object t)
        {
            Text = message;
            Color = col;
            Tag = t;
        }

        public ListBoxItem(string message)
        {
            Text = message;
        }

        public void OnPress(MouseState ms, MouseState lms)
        {
                if (OnMousePressed != null) {
                    OnMousePressed(this, new ListBoxItemPressEventArgs {Ms = ms, Lms = lms});
                }
        }

        public static implicit operator ListBoxItem(string s)
        {
         return new ListBoxItem(s);   
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
        public bool Draggable;
        public int Dragged = -1;
        

        public InteractiveListBox(Rectangle loc, IGameContainer parent) {
            location_ = loc;
            whitepixel_ = parent.whitepixel_;
            font1_ = parent.font1_;
            Parent = parent;
            Parent.AddComponent(this);

            textH = (int)font1_.MeasureString("O").Y;
            textW = (int)font1_.MeasureString("O").X;

            buttonUp_ = new Button(Vector2.Zero, "^", Parent);
            var bup = new Vector2(loc.X + loc.Width - buttonUp_.Width, loc.Y);
            buttonUp_.SetPosition(bup);
            buttonUp_.OnPressed += buttonUp__onPressed;

            buttonDown_ = new Button(Vector2.Zero, "v", Parent);
            var bdow = new Vector2(bup.X, loc.Y + loc.Height - buttonDown_.Height);
            buttonDown_.SetPosition(bdow);
            buttonDown_.OnPressed += buttonDown__onPressed;

            progress_ =
                new VerticalSlider(
                    new Rectangle((int)(bup.X), (int)(bup.Y + buttonUp_.Height), (int)buttonUp_.Width,
                                  (int)(bdow.Y - bup.Y)), parent);
            progress_.OnMove += progress__OnMove;

            Visible = true;
        }

        void progress__OnMove(object sender, EventArgs e) {
            topIndex = progress_.Start;
        }

        private void buttonDown__onPressed(object sender, EventArgs e) {
            topIndex++;
            if (topIndex > Items.Count) {
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
                if (item != Dragged || !Draggable) {
                    Vector2 tp = p + new Vector2(10, bottom);
                    string txt = Items[item].Text;
                    if (aimed == item) {
                        var c = new Rectangle((int) tp.X - 1, (int) tp.Y - 1, txt.Length*textW + 2, textH + 2);
                        DrawBox(sb, c, 1);
                        sb.DrawString(font1_, txt, tp, Items[item].Color);
                    }
                    else {
                        sb.DrawString(font1_, Items[item].Text, p + new Vector2(10, bottom), Items[item].Color);
                    }

                    if (Items[item].Progress > 0) {
                        var c2 = new Rectangle((int) tp.X - 1, (int) tp.Y - 1,
                            (int) ((txt.Length*textW + 2)*((Items[item].Progress > 100) ? 1 : Items[item].Progress/100.0)),textH + 2);
                        sb.Draw(whitepixel_, c2, new Color(0, 0.1f, 0, 0.5f));
                    }
                }

                item++;
                bottom += textH + 2;
            }

            if (Dragged != -1 && Items.Count > 0)
            {
                var tp = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                var txt = Items[Dragged].Text;
                var c = new Rectangle((int)tp.X - 1, (int)tp.Y - 1, txt.Length * textW + 2, textH + 2);
                sb.Draw(whitepixel_, c, new Color(0, 0, 0, 0.5f));
                DrawBox(sb, c, 1);
                sb.DrawString(font1_, txt, tp, Items[Dragged].Color);
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
        private int lastW;
        private bool drag;
        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            var p = GetPosition();
            var ps = p + new Vector2(location_.Width, location_.Height);
            if (ms.X > p.X && ms.Y > p.Y && ms.X < ps.X && ms.Y < ps.Y) {

                if (ms.ScrollWheelValue - lastW < 0) {
                    buttonDown__onPressed(null, null);
                }

                if (ms.ScrollWheelValue - lastW > 0)
                {
                    buttonUp__onPressed(null, null);
                }

                aimed = (int) ((ms.Y - p.Y)/(textH + 2)) + topIndex;
                if (aimed >= Items.Count) {
                    aimed = -1;
                } else if (!(ms.X > p.X + 10) || !(ms.X < p.X + 10 + Items[aimed].Text.Length*textW)) {
                    aimed = -1;
                }

                if (Draggable && aimed != -1 && ms.LeftButton == ButtonState.Pressed && lms.LeftButton == ButtonState.Released) {
                    drag = true;
                    Dragged = aimed;
                }

                if (ms.LeftButton == ButtonState.Released) {
                    drag = false;
                    Dragged = -1;
                }

                if (aimed != -1 && (ms.RightButton == ButtonState.Pressed || ms.LeftButton == ButtonState.Pressed)) {
                    Items[aimed].OnPress(ms, lms);
                }

                lastW = ms.ScrollWheelValue;
            }
            else {
                aimed = -1;
            }
        }

        public Vector2 GetPosition() {
            return new Vector2(location_.X, location_.Y) + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            location_ = new Rectangle((int) pos.X, (int) pos.Y, location_.Width, location_.Height);
        }

        public void ScrollBottom() {
            var cur = Items.Count;
            var bot = location_.Height;
            while (true) {
                bot -= textH + 2;
                if (bot > 0) {
                    cur--;
                }
                else {
                    break;
                }
            }
            cur++;
            if (cur < 0) {
                cur = 0;
            }
            topIndex = cur;
        }
    }
} 
