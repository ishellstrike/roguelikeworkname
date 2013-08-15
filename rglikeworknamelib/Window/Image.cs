using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class Image : IGameWindowComponent
    {
        private Vector2 pos_;
        public String Text;
        public Color col_;
        public Texture2D image;
        private Window Parent;

        public bool Visible { get; set; }

        public object Tag { get; set; }

        private readonly SpriteFont font1_;

        public Image(Vector2 p, Texture2D im, Color c, Window win)
        {
            pos_ = p;
            col_ = c;
            image = im;

            Parent = win;
            Parent.AddComponent(this);
            Visible = true;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image, Parent.GetLocation() + pos_, col_);
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms)
        {

        }

        public Vector2 GetLocation()
        {
            return pos_;
        }

        public Vector2 GetPosition() {
            return pos_;
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