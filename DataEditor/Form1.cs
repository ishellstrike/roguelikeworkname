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
using rglikeworknamelib.Dungeon.Level;

namespace DataEditor
{
    public partial class Form1 : Form {
        private ItemDataBase idb;
        private BlockDataBase bdb;

        public Form1()
        {
            InitializeComponent();
            ReloadDb();
            RebuildDb();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void ReloadDb() {
            idb = new ItemDataBase();
            bdb = new BlockDataBase();
        }

        void RebuildDb() {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgc = new DataGridViewColumn();
            dgc.Name = "Id";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "AftId";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "stype";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "Name";
            dgc.Width = 100;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "Descr";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "Wei";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            dgc = new DataGridViewColumn();
            dgc.Name = "Vol";
            dgc.Width = 40;
            dgc.CellTemplate = new DataGridViewTextBoxCell();
            dgc.SortMode = DataGridViewColumnSortMode.Automatic;
            dataGridView1.Columns.Add(dgc);

            foreach (var a in idb.data) {   
                var bb = new DataGridViewRow();
                bb.Cells.AddRange(new DataGridViewTextBoxCell() {
                                                                    Value = a.Key
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.afteruseId
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.stype
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.name
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.description
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.weight
                                                                },
                                  new DataGridViewTextBoxCell() {
                                                                    Value = a.Value.volume
                                                                });
                dataGridView1.Rows.Add(bb);
            }

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReloadDb();
            RebuildDb();
        }
    }
}
