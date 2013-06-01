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
using rglikeworknamelib.Dungeon.Level;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace SchemesEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            bdb = new BlockDataBase();
            fdb = new FloorDataBase();
            sdb = new SchemesDataBase();
            gl =new GameLevel(20, 20);
        }

        private PictureBox[] map;
        private BlockDataBase bdb;
        private SchemesDataBase sdb;
        private FloorDataBase fdb;
        private GameLevel gl;
        SpriteBatch sb;

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
                map[i].MouseClick += Form1_Click;
               // map[i].MouseEnter += Form1_Click;
            }

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory()+@"\Content\Data\Schemes";
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + @"\Content\Data\Schemes";

            listBox1.Items.Clear();
            foreach (var a in bdb.Data) {
                listBox1.Items.Add("id" + a.Key + " mtex" + a.Value.texNo + " -- " + a.Value.name);
            }
        }

        void Form1_Click(object sender, EventArgs e) {
            int y = (int) ((PictureBox) sender).Tag % 30 - camx;
            int x = (int) ((PictureBox) sender).Tag / 30 - camy;
            if (x < gl.rx && y < gl.ry && x >= 0 && y >= 0) {
                gl.CreateBlock(x, y, listBox1.SelectedIndex);
            }
        }

        private int camx, camy;
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 30; i++) {
                for (int j = 0; j < 30; j++) {
                    if (i - camx < gl.rx && j - camy < gl.ry && i - camx >= 0 && j - camy >= 0)
                        map[i * 30 + j].Image = gl.GetId(i - camx, j - camy) == 0 ? imageList1.Images[0] : imageList1.Images[1];
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
            gl = new GameLevel(x, y);
            char[] sep = {' '};
            String[] b = sw.ReadToEnd().Split(sep);
            int[] a = b.Select(int.Parse).ToArray();
            gl.CreateAllMapFromArray(a);
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
            sw.Write("~" + gl.rx + "," + gl.ry + "," + textBox1.Text + "\n");
            for (int i = 0; i < gl.rx*gl.ry - 1; i++) {
                    sw.Write(gl.GetId(i)+" ");
                }
            sw.Write(gl.GetId(gl.rx * gl.ry - 1));

            sw.Write("\n");
            sw.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gl = GameLevel.CreateGameLevel(null, null, null, bdb, fdb, sdb, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
            //foreach (var a in gl.blocks_) {
            //    a.id = 0;
            //}
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            mouseleftdown = true;
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
    }
}
