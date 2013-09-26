using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    public class Light
    {
        public float Power;
        public Color Color;
        public float LightRadius;

        public Vector3 Position;
        public Vector3 GetWorldPosition(Vector2 camera)
        {
            return Position - new Vector3(camera.X, camera.Y, 0);
        }
    }
}