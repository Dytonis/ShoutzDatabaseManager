using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoutzDatabaseManager.Editors;
using System.Data.SqlClient;
using System.Collections;
using ShoutzDatabaseManager.Shared;

namespace ShoutzDatabaseManager
{
    public partial class ContestEditor : Form
    {
        public SqlConnection connection;

        public ContestEditor()
        {
            InitializeComponent();

            connection = Program.HADESConnection;
        }

        DayConfiguration config;

        private void ContestEditor_Closing(object sender, FormClosingEventArgs e)
        {
            Program.UsersEditor = null; 
        }

        public void ConfigAccepted(int value)
        {
            DataTable table = new DataTable();
            config.Close();

            listView1.Items.Clear();

            if (value > 0)
            {
                Services.DataServices.DataFetchArguments Args = new Services.DataServices.DataFetchArguments()
                {
                    Command = "SELECT * FROM dbo.Contests WHERE DTEnd > GETDATE() AND DTEnd >= DATEADD(d, " + value + ", GETDATE()); ",
                    Connection = Program.HADESConnection,
                };

                Services.DataServices.CreateDataServiceMethod(Args, AsyncWorker_RunWorkerCompleted);
            }

            else if (value < 0)
            {
                Services.DataServices.DataFetchArguments Args = new Services.DataServices.DataFetchArguments()
                {
                    Command = "SELECT * FROM dbo.Contests WHERE DTEnd < GETDATE() AND DTEnd >= DATEADD(d, " + value + ", GETDATE());",
                    Connection = Program.HADESConnection,
                };

                Services.DataServices.CreateDataServiceMethod(Args, AsyncWorker_RunWorkerCompleted);
            }
        }
        private void AsyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable table = e.Result as DataTable;

            foreach (DataRow row in table.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    if (i == 0 || i == 1 || i == 5 || i == 6)
                        item.SubItems.Add(row[i].ToString());
                }
                listView1.Items.Add(item);
            }

            listView1.ListViewItemSorter = new ListViewItemComparer(2, System.Windows.Forms.SortOrder.Descending);
            listView1.Sort();
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void queryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config = new DayConfiguration(this);
            config.Show();
        }

        private bool ascending = false;
        private int col;
        private void listView1_ColClick(object sender, ColumnClickEventArgs e)
        {
            if (col == e.Column)
            {
                if (e.Column <= 1)
                {
                    ascending = !ascending;
                    listView1.ListViewItemSorter = new ListViewItemStringComparer(e.Column, ascending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending);                    
                    listView1.Sort();
                }
                else
                {
                    ascending = !ascending;
                    listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, ascending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending);
                    listView1.Sort();
                }
            }
            else
            {
                if (e.Column <= 1)
                {
                    col = e.Column;
                    ascending = true;
                    listView1.ListViewItemSorter = new ListViewItemStringComparer(e.Column, ascending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending);
                    listView1.Sort();
                }
                else
                {
                    col = e.Column;
                    ascending = true;
                    listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, ascending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending);
                    listView1.Sort();
                }
            }

            col = e.Column;
        }

        private void viewEnteredUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string UUID = listView1.FocusedItem.Text;

            if (Program.UsersEditor == null)
            {
                Program.UsersEditor = new UsersEditor(UUID);
                Program.UsersEditor.Show();
            }
            else
            {
                Program.UsersEditor.textBox1.Text = UUID;
                Program.UsersEditor.exec();
            }
        }
    }

    /// <summary>
    /// implements microsoft's data comparers
    /// </summary>
    class ListViewItemComparer : IComparer
    {
        private int col;
        private System.Windows.Forms.SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = System.Windows.Forms.SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, System.Windows.Forms.SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal;
            // Determine whether the type being compared is a date type.
            try
            {
                // Parse the two objects passed as a parameter as a DateTime.
                System.DateTime firstDate =
                        DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
                System.DateTime secondDate =
                        DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
                // Compare the two dates.
                returnVal = DateTime.Compare(firstDate, secondDate);
            }
            // If neither compared object has a valid date format, compare
            // as a string.
            catch
            {
                // Compare the two items as a string.
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                            ((ListViewItem)y).SubItems[col].Text);
            }
            // Determine whether the sort order is descending.
            if (order == System.Windows.Forms.SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
    class ListViewItemStringComparer : IComparer
    {
        private int col;
        private System.Windows.Forms.SortOrder order;
        public ListViewItemStringComparer()
        {
            col = 0;
            order = System.Windows.Forms.SortOrder.Ascending;
        }
        public ListViewItemStringComparer(int column, System.Windows.Forms.SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == System.Windows.Forms.SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}
