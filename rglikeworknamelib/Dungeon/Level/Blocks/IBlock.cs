using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public interface IBlock {
        void Update(TimeSpan ts, Vector2 vector2);
        string Id { get; set; }
        Color Lightness { get; set; }
        bool Explored { get; set; }
        Rectangle Source { get; set; }
        string MTex { get; set; }
    }
}