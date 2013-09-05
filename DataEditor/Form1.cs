using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using rglikeworknamelib.Dungeon.Item;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;

namespace DataEditor
{
    public partial class Form1 : Form {

        public Form1()
        {
            InitializeComponent();
            RebuildDb();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void RebuildDb() {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgc = new DataGridViewColumn();
            dgc.Name = "Id";
            dgc.Width = 60;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            var ttt = typeof (ItemData).GetFields();
            foreach (var field in ttt) {
                DataGridViewColumn dgc2 = new DataGridViewColumn();
                dgc2.Name = field.Name;
                dgc2.Width = 60;
                dgc2.CellTemplate = new DataGridViewTextBoxCell();
                dgc2.SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView1.Columns.Add(dgc2);
            }

            new ItemDataBase();

            foreach (var a in ItemDataBase.Data) {   
                var bb = new DataGridViewRow();
                bb.Cells.Add(new DataGridViewTextBoxCell() {Value = a.Key});
                foreach (var fieldInfo in a.Value.GetType().GetFields()) {
                    bb.Cells.Add(new DataGridViewTextBoxCell() {Value = fieldInfo.GetValue(a.Value)});
                }
                dataGridView1.Rows.Add(bb);
            }

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RebuildDb();
        }
    }
}
