using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib.Dungeon.Particles;

namespace rglikeworknamelib.Dungeon.Level.Blocks {
    [Serializable]
    public class Block : IBlock {
        public string Id { get; set; }
        public Color Lightness { get; set; }
        public bool Explored { get; set; }
        public string Mtex { get; set; }

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

        public virtual void Draw(SpriteBatch sb, Dictionary<string, Texture2D> batlas, Vector2 vector2) {
            Color light = Lightness;
            if (Explored && light == Color.Black)
            {
                light = new Color(40, 40, 40);
            }

            sb.Draw(batlas[Mtex],vector2, light);
        }

        public virtual void Update(TimeSpan ts, Vector2 vector2)
        {
            
        }
    }

    [Serializable]
    class BlockButningBochka : IBlock {
        private TimeSpan sec;

        public string Id { get; set; }
        public Color Lightness { get; set; }
        public bool Explored { get; set; }
        public string Mtex { get; set; }

        Random rand = new Random();

        public void Update(TimeSpan ts, Vector2 vector2)
        {
            sec += ts;
            if (sec.TotalSeconds > 0.2f && Lightness != Color.Black) {
                var p = new Vector2(vector2.X + 16 + rand.Next(-5, 5), vector2.Y + 8);
                var part = new Particle(p, 2) {Scale = 1.5f, ScaleDelta = -0.8f, Life = TimeSpan.FromMilliseconds(1500), Angle = -3.14f/2f, Rotation = rand.Next()*6.28f, Vel = 10, RotationDelta = 1};
                ParticleSystem.AddParticle(part);
                sec = TimeSpan.Zero;
            }
        }

        public void Draw(SpriteBatch sb, Dictionary<string, Texture2D> batlas, Vector2 vector2) {
            Color light = Lightness;
            if (Explored && light == Color.Black)
            {
                light = new Color(40, 40, 40);
            }

            sb.Draw(batlas[Mtex], vector2, light);
        }

    }

    public interface IBlock {
        void Update(TimeSpan ts, Vector2 vector2);
        void Draw(SpriteBatch sb, Dictionary<string, Texture2D> batlas, Vector2 vector2);
        string Id { get; set; }
        Color Lightness { get; set; }
        bool Explored { get; set; }
        string Mtex { get; set; }
    }
}