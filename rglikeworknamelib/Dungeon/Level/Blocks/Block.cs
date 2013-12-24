using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Items;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block {
        private string id_;

        public List<IItem> StoredItems = new List<IItem>();

        public virtual bool IsActive
        {
            get { return false; }
        }

        public string Id {
            get { return id_; }
            set {
                id_ = value;
                Data = BlockDataBase.Data[value];
            }
        }

        public BlockData Data { get; private set; }

        public Color Lightness { get; set; }

        public Rectangle Source { get; private set; }

        private string mTex_;
        public string MTex {
            get { return mTex_; }
            set {
                Source = BlockData.GetSource(value);
                mTex_ = value;
            }
        }

        public bool Inner { get; set; }

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
    }
}