using System;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public struct Stat {
        public float Current, Max;

        public Stat(float a, float b) {
            Current = a;
            Max = b;
        }

        public Stat(float a)
        {
            Current = a;
            Max = a;
        }
    }
}