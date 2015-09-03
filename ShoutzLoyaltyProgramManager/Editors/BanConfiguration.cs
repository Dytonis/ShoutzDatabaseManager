using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoutzLoyaltyProgramManager.Shared;

namespace ShoutzLoyaltyProgramManager.Editors
{
    public partial class BanConfiguration : Form
    {
        private string _UUID;
        public BanConfiguration(string UUID)
        {
            InitializeComponent();

            _UUID = UUID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Services.DataServices.DataFetchArguments Args2 = new Services.DataServices.DataFetchArguments()
            {
                Command = "HubAdmin.BlacklistUser",
                Connection = Program.CERBERUSConnection,
                SqlCommandArgumentNames = new string[] { "UserID", "AdminName", "ReasonForListing", "Category", "EndDate" },
                SqlCommandArguments = new object[] { _UUID, textBox2.Text, textBox1.Text, "Black", textBox3.Text},
                Type = CommandType.StoredProcedure
            };

            Services.DataServices.CreateDataServiceMethod(Args2, Completed);
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Result == null && e.Error != null)
            {
                MessageBox.Show("There was an error while trying to complete this operation: \n\n" + e.Error.Message);
                Close();
            }
            else if (e.Result != null && e.Error == null)
            {
                MessageBox.Show("Success");
                Close();
            }
        }
    }
}
