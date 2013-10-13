using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block : IBlock {
        private string id_;
        public string Id {
            get { return id_; }
            set {
                data_ = BlockDataBase.Data[value];
                id_ = value;
            }
        }

        [NonSerialized] private BlockData data_;
        public BlockData Data
        {
            get { return data_; }
        }

        [NonSerialized]private Color lightness_;
        public Color Lightness {
            get { return lightness_; }
            set { lightness_ = value; }
        }

        public bool Explored { get; set; }

        [NonSerialized]private Rectangle source_;
        public Rectangle Source {
            get { return source_; }
        }

        private string mTex_;
        public string MTex {
            get { return mTex_; }
            set
            {
                source_ = BlockData.GetSource(value);
                mTex_ = value;
            }
        }

        public void OnLoad() {
            source_ = BlockData.GetSource(MTex);
            data_ = BlockDataBase.Data[id_];
        }

        public static string GetSmartActionName(SmartAction smartAction)
        {
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

        public virtual void Update(TimeSpan ts, Vector2 vector2)
        {
            
        }
    }
}