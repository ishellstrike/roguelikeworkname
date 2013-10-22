using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib {
    public static class DrawUlils {
        /// <summary>
        ///     Color setup -- ~100100100Some Text~200200100more text
        ///     ~rrrgggbbb
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <param name="sb"></param>
        public static void DrawColoredString(SpriteFont font, string text, Vector2 pos, SpriteBatch sb, Color col) {
            if (!text.Contains('~')) {
                sb.DrawString(font, text, pos, col);
                return;
            }
            var offseter = new Vector2();
            if (!text.StartsWith("~")) {
                string substring = text.Substring(text.IndexOf('~'));
                sb.DrawString(font, substring, pos, col);
                Vector2 measureString = font.MeasureString(substring);
                offseter.X += measureString.X;
                offseter.Y += measureString.Y - 38;
                text = text.Substring(text.IndexOf('~'));
            }
            string[] t = text.Split('~');

            foreach (string s in t) {
                if (!string.IsNullOrEmpty(s)) {
                    int tr = int.Parse(s.Substring(0, 3));
                    int tg = int.Parse(s.Substring(3, 3));
                    int tb = int.Parse(s.Substring(6, 3));
                    string str = s.Substring(9);
                    sb.DrawString(font, str, pos + offseter, new Color(tr, tg, tb));
                    Vector2 f = font.MeasureString(str);
                    offseter.X += f.X;
                    offseter.Y += f.Y - 38;
                }
            }
        }
    }
}