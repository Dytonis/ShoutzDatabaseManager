using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoutzLoyaltyProgramManager.Shared
{
    public partial class Waiting : Form
    {
        BackgroundWorker _worker;
        public Waiting(BackgroundWorker worker)
        {
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 50;
            _worker = worker;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _worker.CancelAsync();
        }
    }
}
