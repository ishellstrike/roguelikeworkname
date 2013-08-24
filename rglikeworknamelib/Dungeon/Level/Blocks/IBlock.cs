using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    public interface IBlock {
        void Update(TimeSpan ts, Vector2 vector2);
        void Draw(SpriteBatch sb, Dictionary<string, Texture2D> batlas, Vector2 vector2);
        string Id { get; set; }
        Color Lightness { get; set; }
        bool Explored { get; set; }
        string Mtex { get; set; }
    }
}