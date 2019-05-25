using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace Редактор_тестов_на_XML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string FilePath;
        Boolean FileOpened = false;
        Boolean FileRedacted = false;

        [Serializable]
        public class Tests
        {

            
            public class Test
            {
                [XmlAttribute]
                public string TestId { get; set; }
                [XmlAttribute]
                public string TestType { get; set; }
                public string Name { get; set; }
                public string CommandLine { get; set; }
                public string Input { get; set; }
                public string Output { get; set; }
                public Boolean CanBeSkipped { get; set; }

                public Test()
                { }
                public Test(string testId, string testType, string name, string commandLine, string input, string output, Boolean canBeSkipped)
                {
                    TestId = testId;
                    TestType = testType;
                    Name = name;
                    CommandLine = commandLine;
                    Input = input;
                    Output = output;
                    CanBeSkipped = canBeSkipped;
                }
            }

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            FilePath = openFileDialog1.FileName;

            dataGridView1.Rows.Clear();

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;
            XmlSerializer formatter = new XmlSerializer(typeof(Tests.Test[]), xRoot);

            using (FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate))
            {
                Tests.Test[] newtests = (Tests.Test[])formatter.Deserialize(fs);
                foreach (Tests.Test t in newtests)
                {
                    dataGridView1.Rows.Add(t.TestId, t.TestType, t.Name, t.CommandLine, t.Input, t.Output, t.CanBeSkipped);
                }
            }

            FileOpened = true;
            FileRedacted = false;
            сохранитьToolStripMenuItem1.Enabled = false;

        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var column1 = new DataGridViewColumn();
            column1.HeaderText = "TestId"; //текст в шапке
            column1.Width = 50; //ширина колонки
            //column1.ReadOnly = false; //значение в этой колонке нельзя править
            column1.Name = "TestId"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell(); //тип нашей колонки

            var column2 = new DataGridViewColumn();
            column2.HeaderText = "TestType";
            column2.Name = "TestType";
            column2.Width = 70;
            column2.CellTemplate = new DataGridViewTextBoxCell();

            var column3 = new DataGridViewColumn();
            column3.HeaderText = "Name";
            column3.Name = "Name";
            column3.Width = 200;
            column3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            column3.CellTemplate = new DataGridViewTextBoxCell();

            var column4 = new DataGridViewColumn();
            column4.HeaderText = "CommandLine";
            column4.Name = "CommandLine";
            column4.Width = 110;
            column4.CellTemplate = new DataGridViewTextBoxCell();

            var column5 = new DataGridViewColumn();
            column5.HeaderText = "Input";
            column5.Name = "Input";
            column5.Width = 100;
            column5.CellTemplate = new DataGridViewTextBoxCell();

            var column6 = new DataGridViewColumn();
            column6.HeaderText = "Output";
            column6.Name = "Output";
            column6.CellTemplate = new DataGridViewTextBoxCell();

            var column7 = new DataGridViewCheckBoxColumn();
            column7.HeaderText = "CanBeSkipped";
            column7.Name = "CanBeSkipped";
            column7.Width = 110;
            column7.CellTemplate = new DataGridViewCheckBoxCell();

            dataGridView1.Columns.Add(column1);
            dataGridView1.Columns.Add(column2);
            dataGridView1.Columns.Add(column3);
            dataGridView1.Columns.Add(column4);
            dataGridView1.Columns.Add(column5);
            dataGridView1.Columns.Add(column6);
            dataGridView1.Columns.Add(column7);
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;

            dataGridView1.CurrentCell = null;//Изменяем CurrentCell

            XmlSerializer formatter = new XmlSerializer(typeof(Tests.Test[]), xRoot);

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            FilePath = saveFileDialog1.FileName;

            if (System.IO.File.Exists(@FilePath))
            {
                try
                {
                    System.IO.File.Delete(@FilePath);
                }
                catch
                {
                    MessageBox.Show("Файл не был сохранен, файл занят другим процессом.");
                }

            }

            using (FileStream fs = new FileStream(@FilePath, FileMode.OpenOrCreate))
            {

                Tests.Test[] Tests1 = new Tests.Test[dataGridView1.Rows.Count - 1];
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    Tests1[i] = new Tests.Test(
                                                   Convert.ToString(dataGridView1["TestId", i].Value),
                                                   Convert.ToString(dataGridView1["TestType", i].Value),
                                                   Convert.ToString(dataGridView1["Name", i].Value),
                                                   Convert.ToString(dataGridView1["CommandLine", i].Value),
                                                   Convert.ToString(dataGridView1["Input", i].Value),
                                                   Convert.ToString(dataGridView1["Output", i].Value),
                                                   Convert.ToBoolean(dataGridView1["CanBeSkipped", i].Value)
                                              );
                };

                formatter.Serialize(fs, Tests1);
            }   
        }

        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;
            XmlSerializer formatter = new XmlSerializer(typeof(Tests.Test[]), xRoot);
            dataGridView1.CurrentCell = null;//Изменяем CurrentCell
            if (System.IO.File.Exists(@FilePath))
            {
                try
                {
                    System.IO.File.Delete(@FilePath);
                }
                catch
                {
                    MessageBox.Show("Файл не был сохранен, файл занят другим процессом.");
                }

            }

            using (FileStream fs = new FileStream(@FilePath, FileMode.OpenOrCreate))
            {

                Tests.Test[] Tests1 = new Tests.Test[dataGridView1.Rows.Count - 1];
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    Tests1[i] = new Tests.Test(
                                                   Convert.ToString(dataGridView1["TestId", i].Value),
                                                   Convert.ToString(dataGridView1["TestType", i].Value),
                                                   Convert.ToString(dataGridView1["Name", i].Value),
                                                   Convert.ToString(dataGridView1["CommandLine", i].Value),
                                                   Convert.ToString(dataGridView1["Input", i].Value),
                                                   Convert.ToString(dataGridView1["Output", i].Value),
                                                   Convert.ToBoolean(dataGridView1["CanBeSkipped", i].Value)
                                              );
                };

                formatter.Serialize(fs, Tests1);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (FileOpened == true)
                FileRedacted = true;
            сохранитьToolStripMenuItem1.Enabled = true;
        }
    }
}   
