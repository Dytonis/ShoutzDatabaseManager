using System;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using ShoutzDatabaseManager;
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

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
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

        private User user = new User("none", "");
        private bool pole = false;
        private void button1_Click(object sender, EventArgs e)
        {               
            try
            {
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
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

                user = AdministratorData.Adminstrators.GetUserByUsername(textBox1.Text);

                //check version

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/Dytonis/ShoutzDatabaseManager/master/Release/version.txt");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader r = new StreamReader(stream);

                string content = r.ReadToEnd();

                content = content.Replace("\r\n", "");

                Version latest = new Version(content);

                //check to see if the version matches

                if (!SDMVersion.ToString().Equals(latest.ToString()))
                {
                    if (pole == true)
                    {
                        throw new Exception();
                    }

                    DownloadNewVersion();
                    pole = true;
                }
                else
                {
                    Launch(user);
                }

            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Sorry, the username \'" + textBox1.Text + "\' was not found.");
            }
        }

        public void Launch(User user)
        {
            if(user.GetUsername() == "none")
            {
                throw new Exception();
            }

            string Path = Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "sdmf.exe";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            startInfo.Arguments = user.GetPassword();

            Process process = Process.Start(startInfo);
        }

        ShoutzDatabaseManager.Shared.Waiting waiter;
        public void DownloadNewVersion()
        {
            waiter = new ShoutzDatabaseManager.Shared.Waiting();
            waiter.Show();
            WebClient cExe = new WebClient();
            cExe.DownloadFileCompleted += CExe_DownloadFileCompleted;
            cExe.DownloadFileAsync(new Uri("https://github.com/Dytonis/ShoutzDatabaseManager/raw/master/Release/sdmf.exe"), Directory.GetCurrentDirectory() + "\\sdmf.exe");
            WebClient cDll = new WebClient();
            cDll.DownloadFileCompleted += CDll_DownloadFileCompleted;
            cDll.DownloadFileAsync(new Uri("https://github.com/Dytonis/ShoutzDatabaseManager/raw/master/Release/ShoutzDatabaseManager_AdministratorData.dll"), Directory.GetCurrentDirectory() + "\\ShoutzDatabaseManager_AdministratorData.dll");
        }

        private int Progress = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void CheckForCompletion()
        {
            if(Progress >= 2)
            {
                waiter.Close();
            }
        }

        private void CDll_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Progress++;
            CheckForCompletion();
        }

        private void CExe_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Progress++;
            CheckForCompletion();
        }
    }
}
