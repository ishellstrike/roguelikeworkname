using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Parser;

namespace DropListEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Clear();

            new ItemDataBase();

            foreach (var itemData in ItemDataBase.Data) {
                listBox1.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }

            new BlockDataBase();

            foreach (var itemData in BlockDataBase.Storages) {
                listBox2.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if(ItemDataBase.SpawnLists.ContainsKey(BlockDataBase.Storages.ElementAt(listBox2.SelectedIndex).Key)) {
                foreach (var list in ItemDataBase.SpawnLists[BlockDataBase.Storages.ElementAt(listBox2.SelectedIndex).Key]) {
                    sb.Append("x");
                    sb.Append(list.Repeat);
                    sb.Append(" :: ");
                    sb.Append(string.Join(", ", list.Ids));
                    sb.Append("\n       ");
                    sb.Append(list.MinCount);
                    sb.Append("-");
                    sb.Append(list.MaxCount);
                    sb.Append(" : ");
                    sb.Append(list.Prob);
                    sb.Append("%");
                    sb.AppendLine();
                }
            }
            label1.Text = sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object ibj;
            var serializer =new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            serializer.Formatting = Formatting.Indented;

            using (var stream = new StreamReader(Settings.GetItemDataDirectory()+"//food_core.json")) {
                ibj = serializer.Deserialize<Dictionary<string, ItemData>>(new JsonTextReader(stream));
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try {
                StringBuilder sb = new StringBuilder();
                string ss = textBox1.Text;
                sb.Clear();
                sb.AppendLine("{");

                var temp = ss.Split('~');
                foreach (var s in temp) {
                    if (string.IsNullOrEmpty(s)) {
                        continue;
                    }
                    var lines = s.Split('\n').Select(x => x.Trim('\r')).ToList();
                    sb.AppendFormat("  \"{0}\":{{{1}", lines[0].Split(',')[1], Environment.NewLine);
                    foreach (var line in lines.Skip(1)) {
                        if (string.IsNullOrEmpty(line)) {
                            continue;
                        }
                        var par = line.Split('=');
                        int tryInt;
                        bool isint;
                        isint = int.TryParse(par[1], out tryInt);
                        sb.AppendFormat("    \"{0}\":", par[0]);

                        if (par[1].StartsWith("{")) {
                            var extracted = par[1].Substring(1, par[1].Length - 2);
                            IEnumerable<string> arrayextractor = extracted.Split(',').Select(x => x.Trim(' '));
                            string[] ar = arrayextractor.ToArray();
                            sb.Append("[");
                            foreach (var s1 in ar.Take(ar.Length - 1)) {
                                sb.AppendFormat("\"{0}\", ", s1);
                            }
                            sb.AppendFormat("\"{0}\"],{1}", ar[ar.Length - 1], Environment.NewLine);
                            continue;
                        }
                        if (isint) {
                            sb.AppendFormat("{0},\n", tryInt);
                        }
                        else {
                            sb.AppendFormat("\"{0}\",{1}", par[1], Environment.NewLine);
                        }
                    }
                    sb.AppendLine("  },");
                }

                sb.AppendLine("}");

                textBox2.Text = sb.ToString();
            } catch(Exception ex) {
                MessageBox.Show(string.Format("{0} in Parser", ex));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            serializer.Formatting = Formatting.Indented;
            using (var stream = new StreamWriter("2.json")) {
                Dictionary<string, ItemData> temp = new Dictionary<string, ItemData>();
                var t2 = new ItemData {
                    SortType = ItemType.Craft
                };
                temp.Add("sasas", t2);
                temp.Add("sasas2", t2);
                serializer.Serialize(stream, temp);
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            textBox1.Text = new StreamReader(openFileDialog1.FileName, Encoding.UTF8).ReadToEnd();
        }
    }
}
