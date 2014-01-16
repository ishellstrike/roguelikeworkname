using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using IronPython.Hosting;
using jarg;
using Microsoft.Scripting.Hosting;
using Microsoft.Xna.Framework;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
using Point = Microsoft.Xna.Framework.Point;

namespace BScriptArea
{
    public partial class Form1 : Form
    {
        static ScriptRuntime ipy = Python.CreateRuntime();
        private static ScriptEngine ipyScript;
        private Creature cre;
        private Player pl;
        private LevelWorker lw;
        private MapSector ms;
        private GameLevel gl;
        private GameTime gt;
        private InventorySystem ism;
        private Item item;

        public Form1() {
            ipyScript = ipy.GetEngineByFileExtension("py");
            InitializeComponent();

            Thread t = new Thread(Init);
            t.Start();
        }

        private string infostring;
        private bool loadended;
        public void Init() {
            infostring = "Loading assemblies";
            ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Creature)));
            ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Block)));
            ipy.LoadAssembly(System.Reflection.Assembly.GetAssembly(typeof(Item)));

            infostring = "Loading CreatureDataBase";
            new CreatureDataBase();
            infostring = "Loading BlockDataBase";
            new BlockDataBase();
            infostring = "Loading FloorDataBase";
            new FloorDataBase();
            infostring = "Loading ItemDataBase";
            ItemDataBase.Instance.GetType();
            new AchievementDataBase();

            infostring = "Initialization";
            ism = new InventorySystem();
            pl = new Player(null, null, null, ism) {Position = new Vector3(100, 100,0)};
            cre = new Creature() { Id = "zombie1" };
            lw = new LevelWorker();
            lw.Start();
            gl = new GameLevel(null, null, null, null, lw);
            ms = new MapSector(gl, 0, 0);
            gt = new GameTime(new TimeSpan(0, 0, 0, 0, 500),    new TimeSpan(0, 0, 0, 0, 500));
            
            item = ItemFactory.GetInstance("colacan", 10);
            item = ItemFactory.GetInstance("knife", 10);
             item = ItemFactory.GetInstance("otvertka",1);
            ism.AddItem(item);
            pl.Inventory = ism;

            infostring = "Done";
            loadended = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) {
            timer1.Enabled = !timer1.Enabled;

            if (timer1.Enabled) {
                button2.Text = "Stop";
            }
            else {
                button2.Text = "Run";
            }
        }

        private void Run() {
            if (compiled != null)
            {
                try
                {
                    compiled.BehaviorScript(gt, ms, pl, cre, Settings.rnd);
                    gt = new GameTime(gt.TotalGameTime + gt.ElapsedGameTime, new TimeSpan(0,0,0,0,100));
                }
                catch (Exception ex)
                {
                    richTextBox2.Text = ex.ToString();
                    return;
                }

                richTextBox2.Text = "Running successfully "+gt.TotalGameTime.ToString();
            }
        }

        private dynamic compiled;

        private bool creatureMode = true;

        private void SwitchMode() {
            creatureMode = !creatureMode;
            button3.Text = creatureMode ? "now creature" : "now item";
        }

        private void button1_Click(object sender, EventArgs e) {
            compiled = null;
            if (creatureMode) {
                compiled = ipy.UseFile(Settings.GetCreatureDataDirectory() + "\\bs_nothing.py");
                using (var sr = new StreamWriter(Directory.GetCurrentDirectory() + "\\tempcreature.py")) {
                    sr.Write(richTextBox1.Text);
                }
                try {
                    compiled = ipy.UseFile(Directory.GetCurrentDirectory() + "\\tempcreature.py");
                }
                catch (Exception ex) {
                    richTextBox2.Text = ex.ToString();
                    return;
                }
                gt = new GameTime();
                richTextBox2.Text = "No errors";

                if (timer1.Enabled) {
                    button2_Click(null, null);
                }
                pl.Position = new Vector3(100, 100,0);
                cre.Position = new Vector3(0, 0,0);
                cre.IssureOrder();

                try {
                    compiled.BehaviorInit(cre, Settings.rnd);
                }
                catch (Exception ex) {
                    richTextBox2.Text = ex.ToString();
                    return;
                }

                timer1_Tick(null, null);
            }
            else {
                compiled = ipy.UseFile(Settings.GetItemDataDirectory() + "\\is_nothing.py");
                using (var sr = new StreamWriter(Directory.GetCurrentDirectory() + "\\tempitem.py"))
                {
                    sr.Write(richTextBox1.Text);
                }
                try
                {
                    compiled = ipy.UseFile(Directory.GetCurrentDirectory() + "\\tempitem.py");
                }
                catch (Exception ex)
                {
                    richTextBox2.Text = ex.ToString();
                    return;
                }
                gt = new GameTime();
                richTextBox2.Text = "No errors";

                item = ItemFactory.GetInstance("colacan", 1);
                ism .AddItem(item);

                try {
                    compiled.ItemScript(pl, item, Settings.rnd);
                    infostring = compiled.Name();
                }
                catch (Exception ex) {
                    richTextBox2.Text = ex.ToString();
                    return;
                }
            }
            
    }

        public class ClosableMessageBox
        {
            string _caption;
            ClosableMessageBox(string text, string caption)
            {
                _caption = caption;
                MessageBox.Show(text, caption);
            }
            public static ClosableMessageBox Show(string text, string caption)
            {
               return new ClosableMessageBox(text, caption);
            }
            public void Close()
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }

            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        private float zoom = 3;
        private void timer1_Tick(object sender, EventArgs e) {
            if (compiled != null) {
                if (creatureMode) {
                    cre.Update(gt, ms, pl);
                    pictureBox1.Location = new System.Drawing.Point((int) ((cre.Position.X)/zoom + panel1.Width/2f),
                        (int) ((cre.Position.Y)/zoom + panel1.Height/2f));
                    pictureBox2.Location = new System.Drawing.Point((int) (pl.Position.X/zoom + panel1.Width/2f),
                        (int) (pl.Position.Y/zoom + panel1.Height/2f));
                    label2.Location = pictureBox1.Location + new Size(30, 0);
                    label2.Text = cre.Id + ", " + cre.CurrentOrder.Type + ", " + cre.Position;
                    if (cre.CurrentOrder.Target != null) {
                        label2.Text += Environment.NewLine + "player target";
                    }
                    label2.Text += Environment.NewLine + cre.CurrentOrder.Point;
                    label2.Text += Environment.NewLine + cre.CurrentOrder.Value;
                    Run();
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1) {
                return;
            }
            var name = listBox1.Items[listBox1.SelectedIndex].ToString();
            using (var sw = new StreamReader(Settings.GetCreatureDataDirectory()+"\\"+name+".py")) {
                richTextBox1.Text = sw.ReadToEnd();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            lw.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = infostring;

            if (loadended)
            {
                listBox1.Items.Clear();
                foreach (var script in CreatureDataBase.Scripts)
                {
                    listBox1.Items.Add(script.Key);
                }
                loadended = false;

                listBox2.Items.Clear();
                foreach (var script in ItemDataBase.Instance.ItemScripts)
                {
                    listBox2.Items.Add(script.Key);
                }
                loadended = false;
            }
            if (EventLog.log.Count > 0)
            {
                label1.Text = EventLog.log.Last().message;
            }
        }

        private bool down;
        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            down = true;
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            zoom = (trackBar1.Value/10f);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (down) {
                if (pl != null)
                {
                    pl.Position = new Vector3((e.X - panel1.Width / 2) * zoom, (e.Y - panel1.Height / 2) * zoom,0);
                }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e) {
            down = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SwitchMode();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                return;
            }
            var name = listBox2.Items[listBox2.SelectedIndex].ToString();
            using (var sw = new StreamReader(Settings.GetItemDataDirectory() + "\\" + name + ".py"))
            {
                richTextBox1.Text = sw.ReadToEnd();
            }
        }
    }
}
