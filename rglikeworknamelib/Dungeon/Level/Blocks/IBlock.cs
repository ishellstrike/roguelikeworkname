using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public interface IBlock {
        string Id { get; set; }
        BlockData Data { get; }
        Color Lightness { get; set; }
        Rectangle Source { get; }
        string MTex { get; set; }
        bool Inner { get; set; }
        void Update(TimeSpan elapsedGameTime, Vector2 vector2);
    }
}