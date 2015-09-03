using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoutzDatabaseManager.Shared;
using System.Windows.Forms;

namespace ShoutzDatabaseManager.Editors
{
    public partial class UserAdmin : Form
    {
        private string _UUID;

        public UserAdmin(string UUID)
        {
            InitializeComponent();

            textBox2.Text = UUID;
            exec();
        }

        public void exec()
        {
            try
            {
                listView1.Items.Clear();

                textBox2.Text = textBox2.Text.Trim();

                DataTable table = new DataTable();

                Services.DataServices.DataFetchArguments Args2 = new Services.DataServices.DataFetchArguments()
                {
                    Command = "Interface.GetRecentPointsHistoryByUserID",
                    Connection = Program.LACHESISConnection,
                    SqlCommandArgumentNames = new string[] { "UserID" },
                    SqlCommandArguments = new object[] { textBox2.Text.Trim() },
                    Type = CommandType.StoredProcedure
                };

                Services.DataServices.CreateDataServiceMethod(Args2, AsyncWorker_RunWorkerCompleted2);                
            }
            catch
            {
                MessageBox.Show("There was an error processing this request; the string may be in the incorrect format.");
                return;
            }
        }

        private void AsyncWorker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
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
                _UUID = textBox2.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exec();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            Program.Administrator.Dispose();
            Program.Administrator = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BanConfiguration banner = new BanConfiguration(_UUID);
            banner.Show();
        }
    }
}
