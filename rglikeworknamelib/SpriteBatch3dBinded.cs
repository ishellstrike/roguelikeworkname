using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib
{
    class SpriteBatch3dBinded
    {
        public const int ww = 9;


        /// <summary>
        /// Sprite batch must be begined
        /// </summary>
        public static void DrawStringProjected(SpriteBatch sb, Camera cam, Vector3 pos, SpriteFont fnt, string text, Color col) {
            var p = sb.GraphicsDevice.Viewport.Project(pos, cam.ProjectionMatrix, cam.ViewMatrix, Matrix.Identity);
            sb.DrawString(fnt, text, new Vector2(p.X, p.Y), col);
        }

        public static void DrawStringCenteredProjected(SpriteBatch sb, Camera cam, Vector3 pos, SpriteFont fnt, string text, Color col) {
            var off = text.Length/2*ww;
            var p = sb.GraphicsDevice.Viewport.Project(pos, cam.ProjectionMatrix, cam.ViewMatrix, Matrix.Identity);
            sb.DrawString(fnt, text, new Vector2(p.X - off, p.Y), col);
        }

        public static void DrawStringCenteredUppedProjected(SpriteBatch sb, Camera cam, Vector3 pos, SpriteFont fnt, string text, Color col, int upper)
        {
            var off = text.Length / 2 * ww;
            var p = sb.GraphicsDevice.Viewport.Project(pos, cam.ProjectionMatrix, cam.ViewMatrix, Matrix.Identity);
            sb.DrawString(fnt, text, new Vector2(p.X - off, p.Y - upper), col);
        }
    }
}
