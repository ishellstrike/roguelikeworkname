using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Window {
    public interface IGameContainer {
        void CenterComponentHor(IGameComponent a);

        void CenterComponentVert(IGameComponent a);

        void AddComponent(IGameComponent component);

        bool MouseClickCatched { get; set; }

        Vector2 GetPosition();
        void SetPosition(Vector2 pos);
    }
}