using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Window {
    public class LabelFixed : Label {
        private readonly int fixLength;

        public LabelFixed(Vector2 position, string text, Color c, IGameContainer win)
            : base(position, text, c, win) {
            var w = font1_.MeasureString("O").X;
            fixLength = (int)((win.Width - 26) / w);
            Text = text;
        }

        public LabelFixed(Vector2 position, string text, Color c, int fix, IGameContainer win)
            : base(position, text, c, win) {
            fixLength = fix;
            Text = text;
        }

        public LabelFixed(Vector2 position, string text, IGameContainer win) : base(position, text, win) {
            var w = font1_.MeasureString("O").X;
            fixLength = (int)((win.Width - 26) / w);
            Text = text;
        }

        public LabelFixed(Vector2 position, string text, int fix, IGameContainer win)
            : base(position, text, win)
        {
            fixLength = fix;
            Text = text;
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = NormString(value, fixLength); }
        }

        private static string NormString(string s, int fixLength) {
            string result = "";
            var regular = new Regex(string.Format(".{{0,{0}}}(?=\\s|$)", fixLength), RegexOptions.Multiline);
            MatchCollection matches = regular.Matches(s);
            foreach (Match match in matches) {
                result += match.Value.Trim() + "\n";
            }
            return result;
        }
    }
}