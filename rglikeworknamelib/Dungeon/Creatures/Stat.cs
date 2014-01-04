using System;

namespace rglikeworknamelib.Dungeon.Creatures
{
    [Serializable]
    public struct Stat {
        public float Current, Max;

        public Stat(float current, float max) {
            Current = current;
            Max = max;
        }

        public Stat(float sameValues) {
            Current = sameValues;
            Max = sameValues;
        }

        public override string ToString() {
            return string.Format("{0}/{1}", Current, Max);
        }
    }
}