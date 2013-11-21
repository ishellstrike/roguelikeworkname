using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class RunningLabel : Label {
        private readonly int Size;
        private float runStep;

        private float rem;
        public RunningLabel(Vector2 position, string text, Color c, int size, Window win) : base(position, text, c, win) {
            Size = size;
            Text = text + " --- ";
            rem = font1_.MeasureString("O").X;
        }

        public RunningLabel(Vector2 position, string text, int size, Window win) : base(position, text, win) {
            Size = size;
            Text = text + " --- ";
            rem = font1_.MeasureString("O").X;
        }

        public override float Width {
            get {
                int ss = Size > Text.Length - 1 ? Text.Length - 1 : Size;
                return font1_.MeasureString("f").X*ss;
            }
        }

        private Vector2 microoffset = Vector2.Zero;
        public override void Draw(SpriteBatch sb) {
            int offcet = (int) runStep%Text.Length;
            float v = runStep - (int) runStep;
            microoffset.X = - rem*v;
            string line = Shift(Text, offcet, Size);

            if (isHudColored) {
                sb.DrawString(font1_, line, Parent.GetPosition() + pos_ + microoffset, Settings.HudÑolor);
            }
            else {
                sb.DrawString(font1_, line, Parent.GetPosition() + pos_ + microoffset, col_);
            }
        }

        private static string Shift(string text, int offset, int size) {
            int ss = size > text.Length - 1 ? text.Length - 1 : size;
            return (text.Remove(0, offset) + text.Remove(offset)).Substring(0, ss);
        }

        public override void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks,
                                    bool mh) {
            runStep += 5*(float) gt.ElapsedGameTime.TotalSeconds;
            if (runStep > Text.Length) runStep = 0;
            base.Update(gt, ms, lms, ks, lks, mh);
        }
    }
}