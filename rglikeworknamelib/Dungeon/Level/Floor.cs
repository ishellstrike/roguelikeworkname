using System;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Floor {
        private string id_;
        public string Id
        {
            get { return id_; }
            set
            {
                id_ = value;
                data_ = FloorDataBase.Data[value];
            }
        }

        internal string mTex_;
        public string MTex
        {
            get { return mTex_; }
            set
            {
                source_ = FloorData.GetSource(value);
                mTex_ = value;
            }
        }
        [NonSerialized]
        private FloorData data_;
        public FloorData Data
        {
            get { return data_; }
        }
        [NonSerialized]
        private Rectangle source_;
        public Rectangle Source {
            get { return source_; }
        }
    }
}