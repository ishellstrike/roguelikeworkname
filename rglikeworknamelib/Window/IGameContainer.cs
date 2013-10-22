using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Window {
    public interface IGameContainer {
        bool MouseClickCatched { get; set; }
        Texture2D whitepixel_ { get; set; }
        SpriteFont font1_ { get; set; }
        void CenterComponentHor(IGameComponent component);

        void CenterComponentVert(IGameComponent component);

        void AddComponent(IGameComponent component);

        Vector2 GetPosition();
        void SetPosition(Vector2 pos);
    }
}