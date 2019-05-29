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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        List<Test> NewTests = new List<Test> { new Test("", "", "", "", "", "", false) }; //Массив данных тестов
        string FilePath = "";
        Boolean FileOpened = false;
        Boolean FileRedacted = false;

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string OldFilePath = FilePath;
            FilePath = openFileDialog1.FileName;

            //Создание аттрибута для указания новых свойств сериализации
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;
            XmlSerializer formatter = new XmlSerializer(typeof(List<Test>), xRoot);

            using (FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate))
            {
                try
                {
                    dataGridView1.Columns.Clear();
                    NewTests = (List<Test>)formatter.Deserialize(fs);
                    dataGridView1.DataSource = NewTests;
                    DataGridView dataGridView2 = new DataGridView();
                    dataGridView2.Columns.Add(dataGridView1.Columns[1]);
                    dataGridView1.DataSource = null;
                    dataGridView1 = dataGridView2;
                    //dataGridView2.Rows.Add();
                    //DataColumnCollection NewTable;
                    //NewTable = dataGridView1.Columns[1].;
                    //dataGridView1.DataSource = null;
                }
                catch
                {
                    MessageBox.Show("Структура выбранного файла имеет неправильный вид!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FilePath = OldFilePath;
                    return;
                }

                dataGridView1.Columns["TestId"].Width = 50;
                dataGridView1.Columns["TestType"].Width = 70;
                dataGridView1.Columns["Name"].Width = 200;
                dataGridView1.Columns["CommandLine"].Width = 110;
                dataGridView1.Columns["Input"].Width = 100;
                dataGridView1.Columns["CanBeSkipped"].Width = 110;
            }

            dataGridView1.ReadOnly = true;

            FileOpened = true;
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            //var column1 = new DataGridViewColumn();
            //column1.HeaderText = "TestId"; //текст в шапке
            //column1.Width = 50; //ширина колонки
            ////column1.ReadOnly = false; //значение в этой колонке нельзя править
            //column1.Name = "TestId"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
            //column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
            //column1.CellTemplate = new DataGridViewTextBoxCell(); //тип нашей колонки

            //var column2 = new DataGridViewColumn();
            //column2.HeaderText = "TestType";
            //column2.Name = "TestType";
            //column2.Width = 70;
            //column2.CellTemplate = new DataGridViewTextBoxCell();

            //var column3 = new DataGridViewColumn();
            //column3.HeaderText = "Name";
            //column3.Name = "Name";
            //column3.Width = 200;
            //column3.CellTemplate = new DataGridViewTextBoxCell();

            //var column4 = new DataGridViewColumn();
            //column4.HeaderText = "CommandLine";
            //column4.Name = "CommandLine";
            //column4.Width = 110;
            //column4.CellTemplate = new DataGridViewTextBoxCell();

            //var column5 = new DataGridViewColumn();
            //column5.HeaderText = "Input";
            //column5.Name = "Input";
            //column5.Width = 100;
            //column5.CellTemplate = new DataGridViewTextBoxCell();

            //var column6 = new DataGridViewColumn();
            //column6.HeaderText = "Output";
            //column6.Name = "Output";
            //column6.CellTemplate = new DataGridViewTextBoxCell();

            //var column7 = new DataGridViewCheckBoxColumn();
            //column7.HeaderText = "CanBeSkipped";
            //column7.Name = "CanBeSkipped";
            //column7.Width = 110;
            //column7.CellTemplate = new DataGridViewCheckBoxCell();

            //dataGridView1.Columns.Add(column1);
            //dataGridView1.Columns.Add(column2);
            //dataGridView1.Columns.Add(column3);
            //dataGridView1.Columns.Add(column4);
            //dataGridView1.Columns.Add(column5);
            //dataGridView1.Columns.Add(column6);
            //dataGridView1.Columns.Add(column7);

            dataGridView1.DataSource = NewTests;

            dataGridView1.Columns["TestId"].Width = 50;
            dataGridView1.Columns["TestType"].Width = 70;
            dataGridView1.Columns["Name"].Width = 200;
            dataGridView1.Columns["CommandLine"].Width = 110;
            dataGridView1.Columns["Input"].Width = 100;
            dataGridView1.Columns["CanBeSkipped"].Width = 110;

        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileRedacted == true)
            {
                if (MessageBox.Show("Таблица была изменена.\n Сохранить ее?", "Редактор тестов на XML", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (FilePath == "")
                        сохранитьToolStripMenuItem_Click(sender, e);
                        else
                            сохранитьToolStripMenuItem1_Click(sender, e);
                }
            }
            NewTests.Add(new Test("", "", "", "", "", "", false));
            MainForm_Load(sender, e);

            FileOpened = false;
            FileRedacted = false;
            FilePath = "";
            сохранитьToolStripMenuItem1.Enabled = false;
        }
            
     

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Создание аттрибута для указания новых свойств сериализации
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;

            dataGridView1.CurrentCell = null;//Снятие выделений с таблицы

            XmlSerializer formatter = new XmlSerializer(typeof(List<Test>), xRoot);

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

                //Test[] UpdatedTests = new Test[dataGridView1.Rows.Count - 1];
                //for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                //{
                //    UpdatedTests[i] = new Test(
                //                                   Convert.ToString(dataGridView1["TestId", i].Value),
                //                                   Convert.ToString(dataGridView1["TestType", i].Value),
                //                                   Convert.ToString(dataGridView1["Name", i].Value),
                //                                   Convert.ToString(dataGridView1["CommandLine", i].Value),
                //                                   Convert.ToString(dataGridView1["Input", i].Value),
                //                                   Convert.ToString(dataGridView1["Output", i].Value),
                //                                   Convert.ToBoolean(dataGridView1["CanBeSkipped", i].Value)
                //                              );
                //};

                formatter.Serialize(fs, NewTests);
            }

            FileOpened = false;
            FileRedacted = false;
            сохранитьToolStripMenuItem1.Enabled = false;

        }

        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Создание аттрибута для указания новых свойств сериализации
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "Tests";
            xRoot.Namespace = null;
            xRoot.IsNullable = true;
            XmlSerializer formatter = new XmlSerializer(typeof(List<Test>), xRoot);
            dataGridView1.CurrentCell = null;//Снятие выделений с таблицы

            //Задание пути к файлу, если он не указан.
            if (FilePath == "")
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;

            if (System.IO.File.Exists(@FilePath))
            {
                try
                {
                    System.IO.File.Delete(@FilePath);
                }
                catch
                {
                    MessageBox.Show("Файл не был сохранен, файл занят другим процессом.",
                        "Редактор тестов на XML",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    return;
                }

            }

            using (FileStream fs = new FileStream(@FilePath, FileMode.OpenOrCreate))
            {

                Test[] UpdatedTests = new Test[dataGridView1.Rows.Count - 1];
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    UpdatedTests[i] = new Test(
                                                   Convert.ToString(dataGridView1["TestId", i].Value),
                                                   Convert.ToString(dataGridView1["TestType", i].Value),
                                                   Convert.ToString(dataGridView1["Name", i].Value),
                                                   Convert.ToString(dataGridView1["CommandLine", i].Value),
                                                   Convert.ToString(dataGridView1["Input", i].Value),
                                                   Convert.ToString(dataGridView1["Output", i].Value),
                                                   Convert.ToBoolean(dataGridView1["CanBeSkipped", i].Value)
                                              );
                };

                formatter.Serialize(fs, UpdatedTests);
            }

            FileRedacted = false;
            сохранитьToolStripMenuItem1.Enabled = false;

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
                FileRedacted = true;
            сохранитьToolStripMenuItem1.Enabled = true;
        }

        private void файлToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            dataGridView1.CurrentCell = null;//Снятие выделений с таблицы
        }

        private void MainForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
            if (FileRedacted == true)
            {

                switch (MessageBox.Show("Таблица была изменена.\n Сохранить ее?",
                    "Редактор тестов на XML",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                            if (FilePath == "")
                                сохранитьToolStripMenuItem_Click(sender, e);
                            else
                                сохранитьToolStripMenuItem1_Click(sender, e);
                        return;

                    case DialogResult.Cancel:
                            e.Cancel = true;
                        return;
                }
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
           // NewTests[NewTests.Length + 1] = new Test();
        }

        private void добавитьСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTests.Add(new Test("", "", "", "", "", "", false));
            dataGridView1.DataSource = NewTests;
            DataGridView dataGridView2 = new DataGridView();
            dataGridView1.DataSource = null;
            dataGridView1 = dataGridView2;

            //MessageBox.Show(NewTests.Length.ToString());
        }

        private void удалитьСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}   
