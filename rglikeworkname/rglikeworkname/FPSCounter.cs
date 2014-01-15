using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using jarg;

namespace Mork {
    /// <summary>
    ///     Displays the FPS
    /// </summary>
    public static class FrameRateCounter {
        private const int MAX_GR = 300;
        private static TimeSpan elapsedTime_ = TimeSpan.Zero;
        private static int frameCounter_;
        private static int frameRate_;
        private static readonly Vector2 position_ = new Vector2(10, 10);
        private static readonly Vector2 ofs = new Vector2(0, 15);
        private static float memo;
        private static readonly int[] graph = new int[MAX_GR];
        private static int curent_;
        private static int max_ = 1, min_;
        private static readonly int[] insec = new[] {6, 6, 6, 6, 6, 6, 6, 6, 6, 6};
        private static byte curinsec;

        public static void Update(GameTime gameTime) {
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
                //min_ = graph.Min();
                if (max_ - min_ == 0) {
                    min_ -= 1;
                }

                frameCounter_ = 0;
                if (curent_%10 == 0) {
                    memo = (long) (Process.GetCurrentProcess().WorkingSet64/1024f/1024f);
                }
            }
        }

        private static float uav = 0;
        private static float dav = 0;

        public static void Draw(GameTime gameTime, SpriteFont fnt, SpriteBatch sb, LineBatch lb, int resx, int resy,
                                Stopwatch draw, Stopwatch update) {
            frameCounter_++;

            string fps = string.Format("{0}x{1} {2} fps", resx, resy, insec.Sum());
            uav = uav * 0.99f + (float)update.Elapsed.TotalMilliseconds * 0.01f;
            dav = dav * 0.99f + (float)draw.Elapsed.TotalMilliseconds * 0.01f;
            string mem = string.Format("{0} MiB {1}U,D={2:0.00}ms, {3:0.00}ms{1}Uav,Dav={4:0.00}ms, {5:0.00}ms", memo, Environment.NewLine, update.Elapsed.TotalMilliseconds,
                                       draw.Elapsed.TotalMilliseconds, uav, dav);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                     DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
            sb.DrawString(fnt, fps, position_ + Vector2.One, Color.Black);
            sb.DrawString(fnt, fps, position_, Color.White);

            sb.DrawString(fnt, mem, position_ + Vector2.One + ofs, Color.Black);
            sb.DrawString(fnt, mem, position_ + ofs, Color.White);


            var offset = new Vector2(150, 15);
            for (int index = 0; index < MAX_GR; index++) {
                float a = (((float) graph[index])/(max_)*100.0f);
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
                    lb.AddLine(new Vector2(10 + index, 100) + offset, new Vector2(10 + index, 100 - a) + offset, col, 1);
                }
            }

            TimeSpan overal = draw.Elapsed + update.Elapsed;

            var supd = (int) (update.Elapsed.TotalMilliseconds/overal.TotalMilliseconds*60.0);
            int sudraw = (60 - supd);

            for (int i = 0; i < supd; i++) {
                lb.AddLine(new Vector2(10 + i, 130) + offset, new Vector2(10 + i, 100) + offset, Color.DarkBlue, 1);
            }

            for (int i = 0; i < sudraw; i++) {
                lb.AddLine(new Vector2(10 + i + supd, 130) + offset, new Vector2(10 + i + supd, 100) + offset,
                           Color.DarkGreen, 1);
            }

            int average = graph.Sum() / graph.Length;
            sb.DrawString(fnt, string.Concat("average = ", average), new Vector2(10, 115) + offset, Color.Red);

            sb.End();
        }
    }
}