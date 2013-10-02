using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public struct Dress {
        public string id;
        public Color col;
        public Dress(string i , Color c) {
            id = i;
            col = c;
        }
    }
}