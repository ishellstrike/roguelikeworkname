using System;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block : IBlock {
        private string id_;
        [NonSerialized] private Color lightness_;

        public string Id {
            get { return id_; }
            set {
                id_ = value;
                data_ = BlockDataBase.Data[value];
            }
        }

        [NonSerialized]
        private BlockData data_;
        public BlockData Data {
            get { return data_; }
        }

        public Color Lightness {
            get { return lightness_; }
            set { lightness_ = value; }
        }

        [NonSerialized]
        private Rectangle source_;
        public Rectangle Source {
            get { return source_; }
        }

        private string mTex_;
        public string MTex {
            get { return mTex_; }
            set {
                source_ = BlockData.GetSource(value);
                mTex_ = value;
            }
        }

        public void OnLoad() {
            source_ = BlockData.GetSource(MTex);
            data_ = BlockDataBase.Data[id_];
        }

        public virtual void Update(TimeSpan ts, Vector2 vector2) {
        }

        public static string GetSmartActionName(SmartAction smartAction) {
            switch (smartAction) {
                case SmartAction.ActionOpenContainer:
                    return "Осмотреть содержимое";
                case SmartAction.ActionOpenClose:
                    return "Открыть/Закрыть";
                default:
                    return "Осмотреть";
            }
        }

        public void SetLight(Color color) {
            Lightness = color;
        }
    }
}