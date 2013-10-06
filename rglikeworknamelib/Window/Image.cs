using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Image : IGameComponent
    {
        private Vector2 pos_;
        public String Text;
        public Color col_;
        public Texture2D image;
        private IGameContainer Parent;

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public Image(Vector2 position, Texture2D image, Color color, IGameContainer win)
        {
            pos_ = position;
            col_ = color;
            this.image = image;

            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image, Parent.GetPosition() + pos_, col_);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {

        }

        public Vector2 GetLocation()
        {
            return pos_;
        }

        public Vector2 GetPosition() {
            return pos_ + Parent.GetPosition();
        }

        public void SetPosition(Vector2 pos) {
            pos_ = pos;
        }

        public float Width {
            get { return image.Width; }
        }

        public float Height {
            get { return image.Height; }
        }
    }
}