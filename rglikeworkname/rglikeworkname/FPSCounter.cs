using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using jarg;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib;

namespace Mork 
{
    /// <summary>
    /// Displays the FPS
    /// </summary>
    public static class FrameRateCounter 
    {
        private const int MAX_GR = 300;
        private static TimeSpan elapsedTime_ = TimeSpan.Zero, elapsedTime2_ = TimeSpan.Zero;
        private static int frameCounter_, frameCounter2_;
        private static int frameRate_, frameRate2_;
        private static readonly Vector2 position_ = new Vector2(10, 50);
        private static readonly Vector2 position2_ = new Vector2(5, 5);
        private static readonly Vector2 ofs = new Vector2(0, 15);
        private static long memo;
        private static readonly int[] graph = new int[MAX_GR];
        private static int curent_;
        private static int max_ = 1, min_;
        private static readonly int[] insec = new[] {6,6,6,6,6,6,6,6,6,6};
        private static byte curinsec;

        public static void Update(GameTime gameTime)
        {
            if (Settings.DebugInfo) {
                elapsedTime_ += gameTime.ElapsedGameTime;
                if (elapsedTime_ > TimeSpan.FromMilliseconds(100))
                {
                    elapsedTime_ -= TimeSpan.FromMilliseconds(100);
                    frameRate_ = frameCounter_;

                    insec[curinsec] = frameRate_;
                    curinsec++;
                    if (curinsec >= 10)
                    {
                        curinsec = 0;
                    }
                    graph[curent_] = insec.Sum();
                    curent_++;
                    if (curent_ >= MAX_GR)
                    {
                        curent_ = 0;
                    }

                    max_ = graph.Max();
                    //min_ = graph.Min();
                    if (max_ - min_ == 0) min_ -= 1;

                    frameCounter_ = 0;
                    if (curent_ % 10 == 0)
                    {
                        memo = (long)(Process.GetCurrentProcess().WorkingSet64 / 1024f / 1024f);
                    }
                }
            }
            else {
                elapsedTime2_ += gameTime.ElapsedGameTime;
                if (elapsedTime2_ > TimeSpan.FromSeconds(1)) {
                    elapsedTime2_ -= TimeSpan.FromSeconds(1);
                    frameRate2_ = frameCounter2_;
                    frameCounter2_ = 0;
                }
            }
        }

        private static double sr_up;
        private static double sr_draw;

        public static void Draw(GameTime gameTime, SpriteFont fnt, SpriteBatch sb, LineBatch lb, int resx, int resy, Stopwatch draw, Stopwatch update) {
            frameCounter_++;
            frameCounter2_++;

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                     DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);

            if (Settings.DebugInfo) {
                string fps = string.Format("{0}x{1} {2} fps", Settings.Resolution.X, Settings.Resolution.Y, insec.Sum());
                string mem = string.Format("{0} MiB {1}D+U={2:0.00}ms", memo, Environment.NewLine,
                                           draw.Elapsed.TotalMilliseconds + update.Elapsed.TotalMilliseconds);


                sb.DrawString(fnt, fps, position_ + Vector2.One, Color.Black);
                sb.DrawString(fnt, fps, position_, Color.White);

                sb.DrawString(fnt, mem, position_ + Vector2.One + ofs, Color.Black);
                sb.DrawString(fnt, mem, position_ + ofs, Color.White);


                var offset = new Vector2(150, 15);
                for (int index = 0; index < MAX_GR; index++) {
                    var a = (((float) graph[index])/(max_)*100.0f);
                    Color col = Color.Lerp(Color.Blue, Color.Red, (float) (graph[index])/(max_));
                    if (index == curent_ - 1) {
                        col.G = 255;
                        col.B = 255;
                    }
                    else if (index == curent_ - 2) {
                        col.G = 200;
                        col.B = 200;
                    }
                    else if (index == curent_ - 3) {
                        col.G = 150;
                        col.B = 150;
                    }
                    else if (index == curent_ - 4) {
                        col.G = 75;
                        col.B = 75;
                    }

                    if (a > 0) {
                        lb.AddLine(new Vector2(10 + index, 100) + offset, new Vector2(10 + index, 100 - a) + offset, col,
                                   1);
                    }
                }

                TimeSpan overal = draw.Elapsed + update.Elapsed;

                var sdraw = (int) (draw.Elapsed.TotalMilliseconds/overal.TotalMilliseconds*60.0);
                var supd = (int) (60 - sdraw);

                for (int i = 0; i < sdraw; i++) {
                    lb.AddLine(new Vector2(10 + i, 130) + offset, new Vector2(10 + i, 100) + offset, Color.DarkBlue, 1);
                }

                for (int i = 0; i < supd; i++) {
                    lb.AddLine(new Vector2(10 + i + sdraw, 130) + offset, new Vector2(10 + i + sdraw, 100) + offset,
                               Color.DarkGreen, 1);
                }

                int average = graph.Sum();
                average /= graph.Length;
                sb.DrawString(fnt, string.Concat("average = ", average), new Vector2(10, 115) + offset, Color.Red);
            } else {
                string fps = string.Format("{0}", frameRate2_);
                sb.DrawString(fnt, fps, position2_ + Vector2.One, Color.Black);
                sb.DrawString(fnt, fps, position2_, Color.White);
            }

            sb.End();
        }
    }
}