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

namespace ShoutzDatabaseManager
{
    public partial class Main : Form
    {

        public bool Authenticated = false;

        public Main(SqlConnection InitialConnection = null)
        {
            InitializeComponent();

            if (InitialConnection != null)
                InitialConnection.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!Authenticated)
            {
                MessageBox.Show("This instance is not authenticated!", "Authentication Required", MessageBoxButtons.OK);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.ContestEditor = new ContestEditor();
            Program.ContestEditor.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.UsersEditor = new Editors.UsersEditor();
            Program.UsersEditor.Show();
        }
    }
}
