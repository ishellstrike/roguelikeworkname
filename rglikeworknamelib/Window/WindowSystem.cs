using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class WindowSystem
    {
        private static readonly List<Window> Windows = new List<Window>();
        public bool Mopusehook, Keyboardhook;

        public bool Visible { get; set; }

        public object Tag { get; set; }

        private readonly Texture2D whitepixel_;
        private readonly SpriteFont font1_;

        public WindowSystem(Texture2D wp, SpriteFont f1) {
            whitepixel_ = wp;
            font1_ = f1;
        }

        //public Window New 


        private float timer1;
        private float force1;
        private TimeSpan ts1;
        public void Draw(SpriteBatch sb, Effect lig1, GameTime gt) {
            if(ts1.TotalMilliseconds > 500) {
                force1 = 0;
                ts1 = TimeSpan.Zero;
            }

            if (force1 != 0) {
                ts1 += gt.ElapsedGameTime;
                lig1.Parameters["force"].SetValue(force1);
                lig1.Parameters["random1"].SetValue((float) Settings.rnd.NextDouble());
                lig1.Parameters["random2"].SetValue((float) Settings.rnd.NextDouble());
                lig1.Parameters["timer"].SetValue(timer1);
                //lig1.Parameters["desaturation_float"].SetValue(0.5f);
                timer1+=100;

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                         DepthStencilState.None, RasterizerState.CullNone, lig1);
            } else {
                sb.Begin();
            }
            for (int i = 0; i < Windows.Count; i++) {
                var component = Windows[i];
                component.Draw(sb);
            }
            sb.End();
        }

        public void DoGlitch()
        {
            timer1 = 0;
            force1 = 1f;
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            Mopusehook = false;
            Keyboardhook = false;

            if(Windows.Count > 0)
            for (int index = Windows.Count-1; index >= 0; index--)
            {
                var component = Windows[index];
                component.Update(gt, ms, lms, ks, lks, Mopusehook);
            }
        }

        public Vector2 GetLocation()
        {
            return Vector2.Zero;
        }

        public Vector2 GetPosition() {
            return Vector2.Zero;
        }

        public void SetPosition(Vector2 pos) {
               
        }

        public float Width
        {
            get { return 0; }
        }

        public float Height
        {
            get { return 0; }
        }

        internal void ToTop(Window win)
        {
            Windows.Remove(win);
            Windows.Insert(Windows.Count, win);
        }

        public void AddWindow(Window w) {
            Windows.Add(w);
        }

        public bool CloseTop() {
            for (int i = Windows.Count-1; i >= 0; i--) {
                var window = Windows[i];
                if (window.Closable && window.Visible) {
                    window.Visible = false;
                    return true;
                }
            }
            return false;
        }
    }
}