using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public interface IGameComponent {
        float Width { get; }
        float Height { get; }
        bool Visible { get; set; }
        object Tag { get; set; }
        void Draw(SpriteBatch sb);
        void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh);

        Vector2 GetPosition();
        void SetPosition(Vector2 pos);
    }
}