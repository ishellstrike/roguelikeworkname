using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Light {
        public Color Color;
        public float LightRadius;

        public Vector3 Position;
        public float Power;

        public Vector3 GetWorldPosition(Vector2 camera, int SQ) {
            return (Position - new Vector3(camera.X, camera.Y, 0))/SQ;
        }
    }
}