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
            Regex regular = new Regex(string.Format(".{{0,{0}}}(?=\\s|$)", fixLength), RegexOptions.Singleline);
            MatchCollection matches = regular.Matches(s);
            foreach (Match match in matches) result += match.Value.Trim() + "\n";
            return result;
        }

        public LabelFixed(Vector2 p, string s, Texture2D wp, SpriteFont wf, Color c, int fixl, Window win) : base(p, s, wp, wf, c, win) {
            fixLength = fixl;
            Text = s;
        }

        public LabelFixed(Vector2 p, string s, int fixl, Texture2D wp, SpriteFont wf, Window win) : base(p, s, wp, wf, win) {
            fixLength = fixl;
            Text = s;
        }
    }
}