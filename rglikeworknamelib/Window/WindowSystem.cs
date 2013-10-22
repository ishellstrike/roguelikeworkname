using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class WindowSystem {
        private static readonly List<Window> Windows = new List<Window>();
        internal readonly SpriteFont font1_;
        internal readonly Texture2D whitepixel_;
        public bool Keyboardhook;
        public bool Mopusehook;

        public WindowSystem(Texture2D wp, SpriteFont f1) {
            whitepixel_ = wp;
            font1_ = f1;
            Visible = true;
        }

        public bool Visible { get; set; }

        public object Tag { get; set; }

        public float Width {
            get { return 0; }
        }

        public float Height {
            get { return 0; }
        }

        //public Window New 
        public void Draw(SpriteBatch sb, Effect lig1, GameTime gt) {
            sb.Begin();
            for (int i = 0; i < Windows.Count; i++) {
                Window component = Windows[i];
                component.Draw(sb);
            }
            sb.End();
        }

        public void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh) {
            Mopusehook = false;
            Keyboardhook = false;

            if (Windows.Count > 0) {
                for (int index = Windows.Count - 1; index >= 0; index--) {
                    Window component = Windows[index];
                    component.Update(gt, ms, lms, ks, lks, Mopusehook);
                }
            }
        }

        public Vector2 GetLocation() {
            return Vector2.Zero;
        }

        public Vector2 GetPosition() {
            return Vector2.Zero;
        }

        public void SetPosition(Vector2 pos) {
        }

        internal void ToTop(Window win) {
            Windows.Remove(win);
            Windows.Insert(Windows.Count, win);
        }

        public void AddWindow(Window window) {
            window.Id = Windows.Count;
            Windows.Add(window);
        }

        public Window GetWindowById(int id) {
            return Windows.Find(x => x.Id == id);
        }

        public bool CloseTop() {
            for (int i = Windows.Count - 1; i >= 0; i--) {
                Window window = Windows[i];
                if (window.Closable && window.Visible) {
                    window.Visible = false;
                    return true;
                }
            }
            return false;
        }

        public void Clear() {
            Windows.Clear();
        }

        /// <summary>
        ///     On top ID in last element
        /// </summary>
        /// <returns></returns>
        public Tuple<int, bool>[] GetVisibleList() {
            List<Tuple<int, bool>> a =
                Windows.Select(window => new Tuple<int, bool>(window.Id, window.Visible)).ToList();
            a.Add(new Tuple<int, bool>(Windows[Windows.Count - 1].Id, true));
            return a.ToArray();
        }

        public void SetVisibleList(Tuple<int, bool>[] a) {
            if (a.Length != Windows.Count + 1) {
                return;
            }

            for (int i = 0; i < a.Length - 1; i++) {
                Tuple<int, bool> b = a[i];
                GetWindowById(b.Item1).Visible = b.Item2;
            }

            GetWindowById(a.Last().Item1).OnTop();
        }
    }
}