using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using rglikeworknamelib;
using rglikeworknamelib.Generation;
using Label = System.Windows.Forms.Label;

namespace TestArea
{
    public partial class Form1 : Form {
        private int off1, off2;

        public Form1()
        {
            InitializeComponent();
        }

        private Stopwatch sw1 = new Stopwatch();
        private TimeSpan ts,ts1,ts2,ts3,ts4,ts5;
        private void button1_Click(object sender, EventArgs e)
        {
            const int size = 128;
            const double zoom = 0.4;

            sw1.Restart();
            var f1 = MapGenerators.NoiseMap(off1, off2, size, size, zoom);
            ts = sw1.Elapsed;
            GetValue(size, pictureBox1, label1, label2, ref ts1, ts, f1);

            sw1.Restart();
            f1 = MapGenerators.SmoothNoiseMap(MapGenerators.NoiseMap(off1, off2, size, size, zoom));
            ts = sw1.Elapsed;
            GetValue(size, pictureBox2, label3, label4, ref ts2, ts, f1);

            sw1.Restart();
            f1 = MapGenerators.SmoothNoiseMap(MapGenerators.SmoothNoiseMap(MapGenerators.NoiseMap(off1, off2, size, size, zoom)));
            ts = sw1.Elapsed;
            GetValue(size, pictureBox3, label5, label6, ref ts3, ts, f1);

            sw1.Restart();
            f1 = MapGenerators.PostEffect(MapGenerators.NoiseMap(off1, off2, size, size, zoom), 5, 2.5);
            ts = sw1.Elapsed;
            GetValue(size, pictureBox4, label7, label8, ref ts4, ts, f1);

            sw1.Restart();
            f1 = MapGenerators.PostEffect(MapGenerators.SmoothNoiseMap(MapGenerators.SmoothNoiseMap(MapGenerators.NoiseMap(off1, off2, size, size, zoom))), 5, 2.5);
            ts = sw1.Elapsed;
            GetValue(size, pictureBox5, label9, label10, ref ts5, ts, f1);

            //bd = bm.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.ReadWrite,
            //            PixelFormat.Format24bppRgb);

            //ptr = bd.Scan0;

            //bytes = Math.Abs(bd.Stride) * bd.Height;
            //rgbValues = new byte[bytes];

            //a = MapGenerators.NoiseMap(mx, my, size, size, zoom);

            //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            //for (int counter = 0; counter < a.Length; counter++)
            //{
            //    var t = (byte)(a[counter] * 255);
            //    rgbValues[counter * 3] = rgbValues[counter * 3 + 1] = rgbValues[counter * 3 + 2] = t;
            //}

            //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            //bm.UnlockBits(bd);
            //pictureBox2.Image = bm;
            //pictureBox2.Refresh();
        }

        private void GetValue(int size, PictureBox pb, Label l1, Label l2, ref TimeSpan ts1, TimeSpan ts, double[] a)
        {
            Bitmap bm = new Bitmap(size, size);

            BitmapData bd;

            double mx = off1;
            double my = off2;

            bd = bm.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.ReadWrite,
                             PixelFormat.Format24bppRgb);

            IntPtr ptr = bd.Scan0;

            int bytes = Math.Abs(bd.Stride)*bd.Height;
            byte[] rgbValues = new byte[bytes];

            l1.Text = ts.ToString();
            l1.Refresh();
            ts1 = TimeSpan.FromTicks((ts1 + ts).Ticks/2);
            l2.Text = ts1.ToString();


            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int counter = 0; counter < a.Length; counter ++) {
                var t = (byte) (a[counter]*255);
                rgbValues[counter*3] = rgbValues[counter*3 + 1] = rgbValues[counter*3 + 2] = t;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            bm.UnlockBits(bd);
            pb.Image = bm;
            pb.Refresh();
        }

        private bool down;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            down = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            down = false;
        }

        private Point lastpos;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {

            if (down) {
                off1 -= e.X - lastpos.X;
                off2 -= e.Y - lastpos.Y;
                button1_Click(null, null);
            }

            lastpos = e.Location;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(null, null);
            label2.Text = TimeSpan.Zero.ToString();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(512, 512);

            BitmapData bd;

            double mx = off1;
            double my = off2;

            bd = bm.LockBits(new Rectangle(0, 0, 512, 512), ImageLockMode.ReadWrite,
                             PixelFormat.Format24bppRgb);

            IntPtr ptr = bd.Scan0;

            int bytes = Math.Abs(bd.Stride) * bd.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            byte[] a = new byte[512*512];

            var ff = Directory.GetFiles(Settings.GetWorldsDirectory(),"*.rlm");

            foreach (var s in ff) {
                int q, w;
                int.TryParse(s.Substring(s.LastIndexOf('/') + 2, s.IndexOf(',') - 1 - s.LastIndexOf('/') - 2), out q);
                int.TryParse(s.Substring(s.IndexOf(',') + 1, s.IndexOf('.')-1 - s.IndexOf(',') - 1), out w);
                if(q > -255 && w > -255 && q < 256 && w < 256)
                a[(q + 255)*512 + (w + 255)] = 1;
            }

            for (int counter = 0; counter < a.Length; counter++)
            {
                var t = (byte)(a[counter] * 255);
                rgbValues[counter * 3] = rgbValues[counter * 3 + 1] = rgbValues[counter * 3 + 2] = t;
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            bm.UnlockBits(bd);
            pictureBox6.Image = bm;
            pictureBox6.Refresh();
        }

        private void pictureBox6_MouseUp(object sender, MouseEventArgs e) {
            down1 = false;
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (down1)
            {
                off1 -= e.X - lastpos.X;
                off2 -= e.Y - lastpos.Y;
                pictureBox6_Click(null, null);
            }

            lastpos = e.Location;
        }

        private bool down1;
        private void pictureBox6_MouseDown(object sender, MouseEventArgs e) {
            down = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(tabPage2.Visible) {
                pictureBox6_Click(null, null);
            }
        }
    }
}
