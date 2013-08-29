using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Image = System.Drawing.Image;

namespace SchemesEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //gl =new GameLevel(20, 20);
        }

        private PictureBox[] map;
        private SchemesMap gl;
        SpriteBatch sb;
        Dictionary<string, Image> imdict = new Dictionary<string, Image>();

        private bool mouseleftdown;
        private void Form1_Load(object sender, EventArgs e)
        {
            map = new PictureBox[30 * 30];
            for (int i = 0; i < map.Length; i++) {
                map[i] = new PictureBox();
                map[i].Height = 16;
                map[i].Width = 16;
                map[i].Top = 10 + i % 30 * 16;
                map[i].Left = 10 + i / 30 * 16;
                Controls.Add(map[i]);
                map[i].Tag = i;
                map[i].MouseDown += Form1_MouseDown_1;
                map[i].MouseMove += Form1_MouseMove;
                map[i].MouseUp += Form1_MouseUp;
            }

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()+@"\Content\Data\Schemes";
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + @"\Content\Data\Schemes";

            new BlockDataBase();

            gl = new SchemesMap(20, 20);

            imageList1.Images.Clear();
            StreamReader sr = new StreamReader(Settings.GetObjectTextureDirectory() + @"\textureloadorder.ord", Encoding.Default);


            listBox1.Items.Clear();
            foreach (var a in BlockDataBase.Data)
            {
                listBox1.Items.Add("id" + a.Key + " mtex" + a.Value.MTex + " -- " + a.Value.Name);
            }

            while (true)
            {
                string T = sr.ReadLine();
                if(T == null) break;
                string t = T.Substring(0, T.IndexOf(' '));
                string t2 = T.Substring(T.IndexOf(' ') + 1, T.Length - (T.IndexOf(' ') + 1));

                Image tr = Image.FromFile("Content/"+t + ".png").GetThumbnailImage(16,16, null, IntPtr.Zero);
                imdict.Add(t2, tr);
            }

            button3_Click(null,null);
        }

        private int camx, camy;
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 30; i++) {
                for (int j = 0; j < 30; j++) {
                    if (i - camx < gl.rx && j - camy < gl.ry && i - camx >= 0 && j - camy >= 0)
                        map[i * 30 + j].Image = gl.block[i - camx, j - camy].Mtex == null ? imdict["0"] : imdict[gl.block[i - camx, j - camy].Mtex];
                    else map[i*30 + j].Image = imageList2.Images[2];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            StreamReader sw = new StreamReader(openFileDialog1.FileName);
            sw.Read();
            string[] s = sw.ReadLine().Split(',');
            int x = int.Parse(s[0]), y = int.Parse(s[1]);
            textBox1.Text = s[2];
           // gl = new GameLevel(x, y);
            char[] sep = {' '};
            String[] b = sw.ReadToEnd().Split(sep).Select(r=>r.Trim('\n').Trim('\r')).ToArray();
            gl = new SchemesMap(x,y);
            gl.CreateAllMapFromArray(b);
            sw.Close();

            numericUpDown1.Value = x;
            numericUpDown2.Value = y;
        }

        private void button2_Click(object sender, EventArgs e) {
            saveFileDialog1.FileName = textBox1.Text + ".txt";
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
            sw.Write("~" + gl.rx + "," + gl.ry + "," + textBox1.Text + Environment.NewLine);
            for (int i = 0; i < gl.rx * gl.ry - 1; i++)
            {
                    sw.Write(gl.block[i/gl.ry,i%gl.ry].Id.Trim()+" ");
                }
            sw.Write(gl.block[gl.rx - 1, gl.ry - 1].Id.Trim());

            sw.Write("\n");
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e) {
            gl = new SchemesMap((int)numericUpDown1.Value,(int)numericUpDown2.Value);

            for (int index0 = 0; index0 < gl.rx; index0++) {
                for (int index1 = 0; index1 < gl.ry; index1++) {
                    gl.block[index0, index1].Id = "0";
                    gl.block[index0, index1].Mtex = "0";
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            mouseleftdown = false;
        }

        private void button4_Click(object sender, EventArgs e) {
            camx++;
        }

        private void button6_Click(object sender, EventArgs e) {
            camx--;
        }

        private void button5_Click(object sender, EventArgs e) {
            camy--;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            camy++;
        }

        private void Form1_Click(object sender, MouseEventArgs e)
        {
            if ((sender is PictureBox)) {
                int y = (int) ((PictureBox) sender).Tag%30 - camx;
                int x = (int) ((PictureBox) sender).Tag/30 - camy;
                if (x < gl.rx && y < gl.ry && x >= 0 && y >= 0) {

                    gl.block[x, y].Id =
                            listBox1.Items[listBox1.SelectedIndex].ToString().Substring(0, listBox1.Items[
                                                                                            listBox1.SelectedIndex].
                                                                                            ToString().IndexOf(" ")).
                                Substring(2);
                    gl.block[x, y].Mtex = BlockDataBase.Data[gl.block[x, y].Id].MTex;
                }
            }
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {
            mouseleftdown = true;
            Form1_Click(sender, e);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(mouseleftdown)
                Form1_Click(sender, e);
        }
    }

    public class SchemesMap {
        public Block[,] block;
        public int rx, ry;
         public SchemesMap(int x, int y) {
             rx = x;
             ry = y;
             block = new Block[x,y];

             for (int i = 0; i < x; i++) {
                 for (int j = 0; j < y; j++) {
                     block[i,j] = new Block();
                 }
             }
         }

        public void CreateAllMapFromArray(string[] ints) {
            for (int i = 0; i < ints.Length; i++) {
                block[i/ry, i%ry].Id = ints[i];
                block[i/ry, i%ry].Mtex = BlockDataBase.Data[ints[i]].MTex;
            }
        }
    }
}
