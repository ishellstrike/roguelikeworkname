using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block : IBlock {
        public string Id { get; set; }

        [NonSerialized] internal BlockData data;
        public BlockData Data
        {
            get { return data; }
        }
        public Color Lightness { get; set; }
        public bool Explored { get; set; }
        public Rectangle Source { get; set;}
        public string MTex { get; set; }

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