using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using ShoutzDatabaseManager_AdministratorData;

namespace ShoutzDatabaseManager_Launcher
{
    public partial class ShoutzDatabaseManager_Launcher : Form
    {
        private bool NeedsDownload = false;
        private Version SDMVersion = new Version();
        public ShoutzDatabaseManager_Launcher()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;

            if(!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
            {
                NeedsDownload = true;
                label1.Text = "SDM Version: None";
                SDMVersion = new Version();

                DownloadNewVersion();
            }
            else
            {
                AssemblyName SDMAssembly = AssemblyName.GetAssemblyName(Directory.GetCurrentDirectory() + "\\sdmf.exe");
                SDMVersion = SDMAssembly.Version;
                label1.Text = "SDM Version: " + SDMVersion.ToString();
            }

            Assembly LauncherAssembly = Assembly.GetExecutingAssembly();
            AssemblyName LAM = AssemblyName.GetAssemblyName(LauncherAssembly.Location);
            label4.Text = "Launcher Version: " + LAM.Version.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(String.IsNullOrWhiteSpace(textBox1.Text))
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                User user = AdministratorData.Adminstrators.GetUserByUsername(textBox1.Text);

                //check version
                //downloadnewversion
                
                Launch(user);
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Sorry, the username \'" + textBox1.Text + "\' was not found.");
            }
        }

        public void Launch(User user)
        {
            string Path = Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "sdmf.exe";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            startInfo.Arguments = user.GetPassword();

            Process process = Process.Start(startInfo);
        }

        public void DownloadNewVersion()
        {

        }
    }
}
