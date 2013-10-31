using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Creatures {
    [Serializable]
    public struct DressCreatures {
        public Color col;
        public string id;

        public DressCreatures(string i, Color c) {
            id = i;
            col = c;
        }
    }
}