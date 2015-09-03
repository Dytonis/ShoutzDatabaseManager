using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using ShoutzDatabaseManager.Shared;

namespace ShoutzDatabaseManager.Editors
{
    public partial class UsersEditor : Form
    {
        public UsersEditor(string searchParameter)
        {
            InitializeComponent();

            textBox1.Text = searchParameter;
        }

        public UsersEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exec();
        }

        private void UsersEditor_Load(object sender, EventArgs e)
        {
           if(textBox1.Text != String.Empty)
                exec();
        }

        private void UsersEditor_Closing(object sender, FormClosingEventArgs e)
        {
            Program.UsersEditor.Dispose();
            Program.UsersEditor = null;
        }

        public void exec()
        {
            listView1.Items.Clear();

            textBox1.Text = textBox1.Text.Trim();

            DataTable table = new DataTable();

            if (textBox1.Text.Contains("@"))
            {
                Services.DataServices.DataFetchArguments Args1 = new Services.DataServices.DataFetchArguments()
                {
                    Command = "HubAdmin.SearchForPartialName",
                    Connection = Program.CERBERUSConnection,
                    SqlCommandArgumentNames = new string[] { "@SearchFor" },
                    SqlCommandArguments = new object[] { textBox1.Text.Trim() },
                    Type = CommandType.StoredProcedure
                };

                Services.DataServices.CreateDataServiceMethod(Args1, AsyncWorker_RunWorkerCompleted1);
            }
            else
            {
                Services.DataServices.DataFetchArguments Args = new Services.DataServices.DataFetchArguments()
                {
                    Command = "HubAdmin.GetPlayersForClosedContest",
                    Connection = Program.HADESConnection,
                    SqlCommandArgumentNames = new string[] { "@ContestID" },
                    SqlCommandArguments = new object[] { textBox1.Text.Trim() },
                    Type = CommandType.StoredProcedure
                };

                Services.DataServices.CreateDataServiceMethod(Args, AsyncWorker_RunWorkerCompleted);
            }           
        }

        private void AsyncWorker_RunWorkerCompleted1(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable table = e.Result as DataTable;

            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    if(i == 0 || i == 1 || i == 3)
                        item.SubItems.Add(row[i].ToString());
                }
                listView1.Items.Add(item);
            }
        }

        private void AsyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                DataTable table = e.Result as DataTable;

                foreach (DataRow row in table.Rows)
                {
                    ListViewItem item = new ListViewItem(row[0].ToString());
                    for (int i = 1; i < table.Columns.Count; i++)
                    {
                        item.SubItems.Add(row[i].ToString());
                    }
                    listView1.Items.Add(item);
                }
            }
            catch { }
        }

        private void openInAdministratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string UUID = listView1.FocusedItem.Text;

            if (Program.Administrator == null)
            {
                Program.Administrator = new UserAdmin(UUID);
                Program.Administrator.Show();
            }
            else
            {
                Program.Administrator.textBox2.Text = UUID;
                Program.Administrator.exec();
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                exec();
            }
        }
    }
}
