using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Dungeon.Level {
    [Serializable]
    public class Block {

        public string Id {
            get { return Data.Id; }
            set { Data = Registry.Instance.Blocks[value]; }
        }

        public BlockData Data {
            get { return data_; }
            private set { data_ = value; }
        }

        public Vector2 Source {
            get { return source_; }
        }

        /// <summary>
        /// Do not use ',' '~' in serialization
        /// </summary>
        public virtual string Serialize() {
            return null;
        }

        public virtual void Deserialize(string s)
        {
        }

        private ushort mTex_;
        [NonSerialized]
        private BlockData data_;

        private Vector2 source_;

        public string MTex
        {
            get { return Atlases.Instance.MajorIndexesReverse[mTex_]; }
            set
            {
                source_ = Atlases.GetSource(value);
                mTex_ = Atlases.Instance.MajorIndexes[value];
            }
        }

        public virtual void Update(TimeSpan ts, MapSector ms, Player p)
        {
        }

        public static string GetSmartActionName(SmartAction smartAction)
        {
            switch (smartAction)
            {
                case SmartAction.ActionOpenContainer:
                    return "Осмотреть содержимое";
                case SmartAction.ActionOpenClose:
                    return "Открыть/Закрыть";
                default:
                    return "Осмотреть";
            }
        }
    }
}