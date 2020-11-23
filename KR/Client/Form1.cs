using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.ServiceKR;

namespace Client
{
    public partial class Form1 : Form
    {
        Service1Client serv;
        public Form1()
        {
            InitializeComponent();
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker2.Value = dateTimePicker2.MaxDate;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serv = new Service1Client();
            comboBox1.Items.AddRange(serv.GetTables());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                dataGridView1.Columns.Clear();
                var res = new string[0][];
                switch (comboBox1.SelectedIndex) 
                {
                    case 0:
                        res = serv.GetRecords(comboBox1.Text,0,int.MaxValue,new[] { textBox1.Text });
                        break;
                    case 1:
                        res = serv.GetRecords(comboBox1.Text, 0, int.MaxValue, new object[] { textBox1.Text,((double)numericUpDown1.Value),(numericUpDown2.Value>0?((double)numericUpDown2.Value):((double)numericUpDown2.Maximum)) });
                        break;

                    case 2:
                        res = serv.GetRecords(comboBox1.Text, 0, int.MaxValue, new object[] { textBox1.Text,textBox2.Text,dateTimePicker1.Value,dateTimePicker2.Value });
                        break;
                }
                if (res.Length > 0)
                {
                    for (int j = 0; j < res.Length; j++)
                    {
                        for (int i = 0; i < res[0].Length; i++)
                        {
                            if (j == 0)
                            {
                                var cl = new DataGridViewColumn();
                                cl.CellTemplate = new DataGridViewTextBoxCell();
                                cl.HeaderText = res[j][i];
                                dataGridView1.Columns.Add(cl);
                                if(i==res[0].Length-1 && res.Length>1)
                                    dataGridView1.Rows.Add(res.Length-1);
                            }
                            else
                            {
                                dataGridView1.Rows[j-1].Cells[i].Value = res[j][i].ToString();
                            }
                        }
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        panel1.Visible = false;
                        textBox2.Text = "";
                        panel2.Visible = false;
                        numericUpDown1.Value = 0;
                        numericUpDown2.Value = 0;
                        panel3.Visible = false;
                        dateTimePicker1.Value = dateTimePicker1.MinDate;
                        dateTimePicker2.Value = dateTimePicker2.MaxDate;
                        break;
                    case 1:
                        panel3.Visible = true;
                        panel1.Visible = false;
                        textBox2.Text = "";
                        panel2.Visible = false;
                        numericUpDown1.Value = 0;
                        numericUpDown2.Value = 0;
                        dateTimePicker1.Value = dateTimePicker1.MinDate;
                        dateTimePicker2.Value = dateTimePicker2.MaxDate;
                        break;
                    case 2:
                        panel1.Visible = true;
                        panel2.Visible = true;
                        panel3.Visible = false;
                        numericUpDown1.Value = 0;
                        numericUpDown2.Value = 0;
                        break;
                }
            }
        }
    }
}
