using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public interface IBlock {
        string Id { get; set; }
        BlockData Data { get; }
        Color Lightness { get; set; }
        bool Explored { get; set; }
        Rectangle Source { get; }
        string MTex { get; set; }
        void Update(TimeSpan ts, Vector2 vector2);
        void OnLoad();
    }
}