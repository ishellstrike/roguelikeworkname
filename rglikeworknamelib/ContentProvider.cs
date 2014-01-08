using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using NLog;

namespace rglikeworknamelib
{
    public class ContentProvider
    {
        private static int total;
        private static Texture2D error;
        private static GraphicsDevice GraphicsDevice;
        static readonly Logger Logger = LogManager.GetLogger("ContentProvider");

        public static void Init(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            error = LoadTexture(@"Textures\Blocks\error");
        }

        public static Texture2D LoadTexture(string s)
        {
            Texture2D tex;
            try
            {
                using (var st = File.OpenRead(@"Content/" + s + ".png"))
                {
                    tex = Texture2D.FromStream(GraphicsDevice, st);
                }
            }
            catch (FileNotFoundException) {
                Logger.Error("{0} texture is missed", s);
                total++;
                return error;
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
