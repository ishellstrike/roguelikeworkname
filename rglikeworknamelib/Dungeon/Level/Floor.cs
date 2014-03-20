using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Floor {
        public string Id
        {
            get { return Data.Id; }
            set { data_ = Registry.Instance.Floors[value]; }
        }

        internal ushort mTex_;
        public string MTex
        {
            get { return Atlases.Instance.MajorIndexesReverse[mTex_]; }
            set
            {
                source_ = Atlases.GetSource(value);
                mTex_ = Atlases.Instance.MajorIndexes[value];
            }
        }
        [NonSerialized]
        private FloorData data_;
        public FloorData Data
        {
            get { return data_; }
        }
        [NonSerialized]
        private Vector2 source_;
        public Vector2 Source {
            get { return source_; }
        }
    }
}