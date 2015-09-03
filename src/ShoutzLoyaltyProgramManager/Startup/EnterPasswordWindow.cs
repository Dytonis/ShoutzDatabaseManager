using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoutzDatabaseManager.Shared;

namespace ShoutzDatabaseManager
{
    public partial class EnterPasswordWindow : Form
    {
        protected const string Password = "sandst0ne";

        Splash spawn;

        public EnterPasswordWindow(Splash _spawn)
        {
            InitializeComponent();
            spawn = _spawn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            spawn.PasswordReturn(Enums.PasswordState.Canceled);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == Password)
            {
                spawn.PasswordReturn(Enums.PasswordState.Accepted);
            }
            else
            {
                spawn.PasswordReturn(Enums.PasswordState.Denied);
            }
        }

        public void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button2_Click(null, null);
        }
    }
}
