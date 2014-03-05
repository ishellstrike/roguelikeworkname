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
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Level.Blocks;
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

            new BuffDataBase();


            foreach (var itemData in Registry.Instance.Items)
            {
                listBox1.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }



            foreach (var itemData in Registry.Instance.Blocks)
            {
                listBox2.Items.Add(string.Format("id {0} -- {1}", itemData.Key, itemData.Value.Name));
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var list in Registry.Instance.Blocks.ElementAt(listBox2.SelectedIndex).Value.ItemSpawn)
            {
                    sb.Append("x");
                    sb.Append(list.Repeat);
                    sb.Append(" :: ");
                    sb.Append(string.Join(", ", list.Ids));
                    sb.Append("\n       ");
                    sb.Append(list.Min);
                    sb.Append("-");
                    sb.Append(list.Max);
                    sb.Append(" : ");
                    sb.Append(list.Prob);
                    sb.Append("%");
                    sb.AppendLine();
                }
            sb.AppendLine();

            Block a = BlockFactory.GetInstance(Registry.Instance.Blocks.ElementAt(listBox2.SelectedIndex).Key);
            var storage = a as IItemStorage;
            if (storage != null) {
                Registry.TrySpawnItems(Settings.rnd, a);
                List<Item> itemList = storage.ItemList;
                Registry.StackSimilar(ref itemList);

                foreach (var it in storage.ItemList) {
                    sb.AppendFormat("{1} x{2}{0}", Environment.NewLine, it.Id, it.Count);
                }
            }

            sb.AppendFormat("");
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
                var temp = new List<KeyValuePair<string, ItemData>>();
                var t2 = new ItemData {
                    SortType = ItemType.Craft
                };
                temp.Add(new KeyValuePair<string, ItemData>("sasas", t2));
                temp.Add(new KeyValuePair<string, ItemData>("sasas2", t2));
                serializer.Serialize(stream, temp);
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e) {
            textBox1.Text = new StreamReader(openFileDialog1.FileName, Encoding.Default).ReadToEnd();
        }
    }
}
