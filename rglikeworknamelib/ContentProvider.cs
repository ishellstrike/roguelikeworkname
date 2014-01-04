using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace rglikeworknamelib
{
    public class ContentProvider
    {
        private static int total;
        private static Texture2D error;
        private static GraphicsDevice GraphicsDevice;

        public static void Init(GraphicsDevice GraphicsDevice)
        {
            ContentProvider.GraphicsDevice = GraphicsDevice;
            error = LoadTexture(@"Textures\Blocks\error.png");
        }

        public static Texture2D LoadTexture(string s)
        {
            Texture2D tex = null;
            try
            {
                using (var st = File.OpenRead(@"Content/" + s + ".png"))
                {
                    tex = Texture2D.FromStream(GraphicsDevice, st);
                }
            }
            catch (FileNotFoundException ex)
            {

            }
            total++;
            return tex;
        }

        public static int TotalLoaded()
        {
            return total;
        }
    }
}
