using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using jarg;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mork 
{
    /// <summary>
    /// Displays the FPS
    /// </summary>
    public static class FrameRateCounter 
    {
        private const int MAX_GR = 300;
        private static TimeSpan elapsedTime_ = TimeSpan.Zero;
        private static int frameCounter_;
        private static int frameRate_;
        private static readonly Vector2 position_ = new Vector2(10, 50);
        private static readonly Vector2 ofs = new Vector2(0, 12);
        private static long memo;
        private static readonly int[] graph = new int[MAX_GR];
        private static int curent_;
        private static int max_ = 1, min_;
        private static readonly int[] insec = new int[10];
        private static byte curinsec;

        public static void Update(GameTime gameTime) 
        {
            elapsedTime_ += gameTime.ElapsedGameTime;

            if (elapsedTime_ > TimeSpan.FromMilliseconds(100)) {
                elapsedTime_ -= TimeSpan.FromMilliseconds(100);
                frameRate_ = frameCounter_;

                insec[curinsec] = frameRate_;
                curinsec++;
                if (curinsec >= 10) {
                    curinsec = 0;
                }
                graph[curent_] = insec.Sum();
                curent_++;
                if (curent_ >= MAX_GR) {
                    curent_ = 0;
                }

                max_ = graph.Max();
                min_ = graph.Min();

                frameCounter_ = 0;
                if (curent_ % 10 == 0) {
                    memo = (long) (Process.GetCurrentProcess().WorkingSet64 / 1024f / 1024f);
                }
            }
        }

        public static void Draw(GameTime gameTime, SpriteFont fnt, SpriteBatch sb, LineBatch lb, int resx, int resy) {
            frameCounter_++;

            string fps = string.Format("{0}x{1} {2} fps", resx, resy, insec.Sum());
            string mem = string.Format("{0} MiB", memo);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                     DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
            sb.DrawString(fnt, fps, position_ + Vector2.One, Color.Black);
            sb.DrawString(fnt, fps, position_, Color.White);

            sb.DrawString(fnt, mem, position_ + Vector2.One + ofs, Color.Black);
            sb.DrawString(fnt, mem, position_ + ofs, Color.White);


            var offset = new Vector2(150, 15);
            for (int index = 0; index < MAX_GR; index++) {
                var a = (int) (((float) graph[index]) / (max_) * 100);
                Color col = Color.Lerp(Color.Blue, Color.Red, (float) graph[index] / (max_));
                if (index == curent_ - 1) {
                    col.G = 255;
                    col.B = 255;
                }
                if (index == curent_ - 2) {
                    col.G = 200;
                    col.B = 200;
                }
                if (index == curent_ - 3) {
                    col.G = 150;
                    col.B = 150;
                }
                if (index == curent_ - 4) {
                    col.G = 75;
                    col.B = 75;
                }

                if (a > 0) {
                    lb.AddLine(new Vector2(10 + index, 100) + offset, new Vector2(10 + index, 100 - a) + offset, col, 1);
                }
            }

            int average = graph.Sum();
            average /= graph.Length;
            sb.DrawString(fnt, string.Concat("average = ", average), new Vector2(10, 115) + offset, Color.Red);

            sb.End();
        }
    }
}