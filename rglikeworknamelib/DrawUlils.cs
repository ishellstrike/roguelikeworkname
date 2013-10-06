using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib
{
    public static class DrawUlils
    {
        /// <summary>
        /// Color setup -- ~100100100Some Text~200200100more text
        /// ~rrrgggbbb
        /// </summary>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <param name="sb"></param>
        public static void DrawColoredString(SpriteFont font, string text, Vector2 pos, SpriteBatch sb, Color col) {
            if(!text.Contains('~')) {
                sb.DrawString(font, text, pos, col);
                return;
            }
            Vector2 offseter = new Vector2();
            if (!text.StartsWith("~")) {
                string substring = text.Substring(text.IndexOf('~'));
                sb.DrawString(font, substring, pos, col);
                Vector2 measureString = font.MeasureString(substring);
                offseter.X += measureString.X;
                offseter.Y += measureString.Y - 38;
                text = text.Substring(text.IndexOf('~'));
            }
            var t = text.Split('~');
            
            foreach (var s in t) {
                if (!string.IsNullOrEmpty(s)) {
                    var tr = int.Parse(s.Substring(0, 3));
                    var tg = int.Parse(s.Substring(3, 3));
                    var tb = int.Parse(s.Substring(6, 3));
                    var str = s.Substring(9);
                    sb.DrawString(font, str, pos + offseter, new Color(tr, tg, tb));
                    var f = font.MeasureString(str);
                    offseter.X += f.X;
                    offseter.Y += f.Y - 38;
                }
            }
        }
    }
}
