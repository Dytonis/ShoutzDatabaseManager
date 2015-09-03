using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoutzDatabaseManager.Editors
{
    public partial class DayConfiguration : Form
    {
        ContestEditor _parent;

        public DayConfiguration(ContestEditor parent)
        {
            InitializeComponent();

            _parent = parent;
        }

        private void DayConfiguration_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(textBox1.Text);


                _parent.ConfigAccepted(value);
            }
            catch(Exception er)
            {
                MessageBox.Show("The input is not an integer", "Error", MessageBoxButtons.OK);
                textBox1.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _parent.Close();
            Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                button2_Click(null, null);
        }
    }
}
