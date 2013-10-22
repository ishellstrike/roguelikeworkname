using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public struct Dress {
        public Color col;
        public string id;

        public Dress(string i, Color c) {
            id = i;
            col = c;
        }
    }
}