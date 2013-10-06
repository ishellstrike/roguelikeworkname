using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace rglikeworknamelib.Window {
    public class RunningLabel : Label {
        private float runStep;
        private int Size;
        public RunningLabel(Vector2 position, string text, Color c, int size, Window win) : base(position, text, c, win) {
            Size = size;
            Text = text + " --- ";
        }

        public RunningLabel(Vector2 position, string text, int size, Window win) : base(position, text, win) {
            Size = size;
            Text = text + " --- ";
        }

        public override void Draw(SpriteBatch sb) {

            var offcet = (int)runStep % Text.Length;
            var line = Shift(Text, offcet, Size);

            if (isHudColored)
            {
                sb.DrawString(font1_, line, Parent.GetPosition() + pos_, Settings.HudÑolor);
            }
            else
            {
                sb.DrawString(font1_, line, Parent.GetPosition() + pos_, col_);
            }
                
        }

        static string Shift(string text, int offset, int size) {
            var ss = size > text.Length - 1 ? text.Length - 1 : size;
            return (text.Remove(0, offset) + text.Remove(offset)).Substring(0,ss);
        }

        public override void Update(GameTime gt, MouseState ms, MouseState lms, KeyboardState ks, KeyboardState lks, bool mh)
        {
            runStep += 5*(float)gt.ElapsedGameTime.TotalSeconds;
            base.Update(gt, ms, lms, ks, lks, mh);
        }

        public override float Width
        {
            get 
            {
                var ss = Size > Text.Length - 1 ? Text.Length - 1 : Size;
                return font1_.MeasureString("f").X*ss;
            }
        }
    }
}