using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Window {
    public interface IGameContainer {
        void CenterComponentHor(IGameComponent component);

        void CenterComponentVert(IGameComponent component);

        void AddComponent(IGameComponent component);

        bool MouseClickCatched { get; set; }
        Texture2D whitepixel_ { get; set; }
        SpriteFont font1_ { get; set; }

        Vector2 GetPosition();
        void SetPosition(Vector2 pos);
    }
}