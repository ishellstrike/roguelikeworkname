using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib.Window {
    public class LabelFixed : Label {
        private int fixLength;

        public override sealed string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = NormString(value, fixLength);
            }
        }

        private static string NormString(string s, int fixLength) {
            string result = "";
            Regex regular = new Regex(string.Format(".{{0,{0}}}(?=\\s|$)", fixLength), RegexOptions.Multiline);
            MatchCollection matches = regular.Matches(s);
            foreach (Match match in matches) result += match.Value.Trim() + "\n";
            return result;
        }

        public LabelFixed(Vector2 position, string text, Color c, int fixl, IGameContainer win)
            : base(position, text, c, win)
        {
            fixLength = fixl;
            Text = text;
        }

        public LabelFixed(Vector2 position, string text, int fixl, IGameContainer win) : base(position, text, win) {
            fixLength = fixl;
            Text = text;
        }
    }
}