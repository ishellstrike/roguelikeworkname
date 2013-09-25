using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    class TextBox : IGameComponent
    {
        private Rectangle locate_;
        public String Text="";
        private bool aimed_;
        private IGameContainer Parent;

        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;

        public TextBox(Vector2 p, int length, Texture2D wp, SpriteFont wf, IGameContainer pa)
        {
            whitepixel_ = wp;
            font1_ = wf;
            locate_.X = (int)p.X;
            locate_.Y = (int)p.Y;
            locate_.Height = 20;
            locate_.Width = length;
            Parent = pa;
            Parent.AddComponent(this);
            Visible = true;
        }

        public void Draw(SpriteBatch sb) {
            if (Visible)
            {
                Vector2 realpos = GetPosition();

                Color col = !aimed_ ? Settings.HudÑolor : Color.White;

                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width, 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.Right, locate_.Y) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(2, locate_.Height + 2), SpriteEffects.None, 0);
                sb.Draw(whitepixel_, new Vector2(locate_.X, locate_.Bottom) + Parent.GetPosition(), null, col, 0,
                        Vector2.Zero, new Vector2(locate_.Width + 2, 2), SpriteEffects.None, 0);

                sb.DrawString(font1_, Text, realpos + new Vector2(5, 0), col);
            }
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            var a = ks.GetPressedKeys()[0];
            if (!lks.IsKeyDown(a)) {
                switch (a) {
                    case Keys.Back:
                        if(Text.Length > 0) {
                            Text.Remove(Text.Length - 2);
                        }
                        break;
                    default:
                        Text += a.ToString();
                        break;

                }
            }
        }

        public Vector2 GetPosition() {
            return new Vector2(locate_.X, locate_.Y);
        }

        public void SetPosition(Vector2 pos) {
            locate_.X = (int)pos.X;
            locate_.Y = (int)pos.Y;
        }

        private float width;
        public float Width {
            get { return width; }
        }

        private float height;
        public float Height {
            get { return height; }
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }
    }
}