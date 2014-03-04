using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using rglikeworknamelib;
using rglikeworknamelib.Dungeon.Items;

namespace DataEditor {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            RebuildDb();
        }

        private void Form1_Load(object sender, EventArgs e) {
        }

        private void RebuildDb() {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            var dgc = new DataGridViewColumn();
            dgc.Name = "Id";
            dgc.Width = 60;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            FieldInfo[] ttt = typeof (ItemData).GetFields();
            foreach (FieldInfo field in ttt) {
                var dgc2 = new DataGridViewColumn();
                dgc2.Name = field.Name;
                dgc2.Width = 60;
                dgc2.CellTemplate = new DataGridViewTextBoxCell();
                dgc2.SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView1.Columns.Add(dgc2);
            }

            foreach (var a in Registry.Instance.Items)
            {
                var bb = new DataGridViewRow();
                bb.Cells.Add(new DataGridViewTextBoxCell {Value = a.Key});
                foreach (FieldInfo fieldInfo in a.Value.GetType().GetFields()) {
                    bb.Cells.Add(new DataGridViewTextBoxCell {Value = fieldInfo.GetValue(a.Value)});
                }
                dataGridView1.Rows.Add(bb);
            }

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void button1_Click(object sender, EventArgs e) {
            RebuildDb();
        }
    }
}