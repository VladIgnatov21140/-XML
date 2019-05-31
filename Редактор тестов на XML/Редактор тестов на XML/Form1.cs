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

        List<Test> NewTests = new List<Test> {new Test("", "", "", "", "", "", false) }; //Список данных тестов с новой пустой строкой
        BindingSource DataBinding = new BindingSource();
        string FilePath = "";
        Boolean FileOpened = false;
        Boolean FileRedacted = false;

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
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
                    return;
            }

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
                    NewTests = (List<Test>)formatter.Deserialize(fs);
                    DataBinding.DataSource = NewTests;
                    dataGridView1.DataSource = DataBinding;
                } catch
                    {
                        MessageBox.Show("Структура выбранного файла имеет неправильный вид!", "Не удалось открыть файл", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            FileOpened = true;
            FileRedacted = false;
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Подключение данных из NewTests
            DataBinding.DataSource = NewTests;
            dataGridView1.DataSource = DataBinding;

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

            dataGridView1.DataSource = null;
            NewTests.RemoveRange(0, NewTests.Count - 1);
            NewTests[0] = new Test("", "", "", "", "", "", false);
            dataGridView1.DataSource = DataBinding;

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
                } catch
                    {
                        MessageBox.Show("Файл не был сохранен, файл занят другим процессом.");
                    }

            }

            using (FileStream fs = new FileStream(@FilePath, FileMode.OpenOrCreate))
            {
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
                } catch
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
                formatter.Serialize(fs, NewTests);
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

        private void добавитьСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выделите строку или ячейку строки для добавления строки сверху. Дополнительная строка снизу не считается.", "Не получилось добавить строку.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            } else
                {
                    Int32 SelectedRowIndex = dataGridView1.CurrentRow.Index;
                    //dataGridView1.CurrentCell = null;//Снятие выделений с таблицы
                    dataGridView1.DataSource = null;
                    NewTests.Insert(SelectedRowIndex, new Test("", "", "", "", "", "", false));
                    NewTests.Insert(SelectedRowIndex, new Test("", "", "", "", "", "", false));
                    dataGridView1.DataSource = NewTests;
                    dataGridView1.Update();

                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    NewTests.RemoveAt(SelectedRowIndex);
                    dataGridView1.DataSource = DataBinding;
                }

            FileRedacted = true;
        }

        private void удалитьСтрокуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Int32 SelectedRowIndex = dataGridView1.CurrentRow.Index;
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                NewTests.RemoveAt(SelectedRowIndex);
                dataGridView1.DataSource = DataBinding;
                FileRedacted = true;
            } catch
                {
                    dataGridView1.DataSource = DataBinding;
                    MessageBox.Show("Выделите строку или ячейку строки для ее удаления. Дополнительная строка снизу не считается.", "Не получилось удалить строку.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
        }
    }
}   
