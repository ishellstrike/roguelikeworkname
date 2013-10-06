using System;

namespace rglikeworknamelib.Creatures {
    [Serializable]
    public struct Stat {
        public float Current, Max;

        public Stat(float current, float max) {
            Current = current;
            Max = max;
        }

        public Stat(float sameValues)
        {
            Current = sameValues;
            Max = sameValues;
        }
    }
}