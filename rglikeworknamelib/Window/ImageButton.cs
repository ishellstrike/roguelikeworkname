using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Window {
    public class ImageButton : Button {
        private Texture2D Im;
        public ImageButton(Vector2 p, string s, Texture2D wp, Texture2D im, SpriteFont wf, Window ow) : base(p, s, wp, wf, ow) {
            Im = im;
        }
        public new virtual void Draw(SpriteBatch sb) {
            base.Draw(sb);
        }
    }
}