using System;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace ShoutzDatabaseManager_Launcher
{
    public partial class ShoutzDatabaseManager_Launcher : Form
    {
        private Version SDMVersion = new Version();
        private Version OldVersion = new Version();
        public ShoutzDatabaseManager_Launcher()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;

                if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
                {
                    label1.Text = "SDM Version: None";
                    SDMVersion = new Version();
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
            catch(BadImageFormatException)
            {
                MessageBox.Show("The file 'sdmf.exe' is corrupt. The launcher will delete it and attempt to re-download. If this is not the first time you have seen this message in a row without modifying " +
                    "sdmf.exe or any .dll then please contact tanner@shoutz about error code 2.\n\nIf re-starting the program does not work, you can still manually launch the manager. \n1. Navigate to "
                    + "https://github.com/Dytonis/ShoutzDatabaseManager/tree/master/Release" + " and download every .dll and sdmf.exe.\n" 
                    + "2. Move each file you have downloaded into a folder and run sdmf.exe", "Error code 2");

                File.Delete(Directory.GetCurrentDirectory() + "\\sdmf.exe");
                Close();
            }
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

        private bool pole = false;
        private void button1_Click(object sender, EventArgs e)
        {
            OldVersion = SDMVersion;

            try
            {
                if(!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
                {
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
                    Launch();
                    Close();
                }

            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Sorry, the username \'" + textBox1.Text + "\' was not found.");
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem trying to request the current version. Please check your internet connection and try again. The github repository may be down.");
            }
        }

        public void Launch()
        {
            string Path = Directory.GetCurrentDirectory();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "sdmf.exe";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;

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
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/Dytonis/ShoutzDatabaseManager/master/Release/version.txt");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader r = new StreamReader(stream);

                string content = r.ReadToEnd();

                content = content.Replace("\r\n", "");

                Version latest = new Version(content);

                if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
                {
                    label1.Text = "SDM Version: None";
                    SDMVersion = new Version();

                    DownloadNewVersion();
                }
                else
                {
                    AssemblyName SDMAssembly = AssemblyName.GetAssemblyName(Directory.GetCurrentDirectory() + "\\sdmf.exe");
                    SDMVersion = SDMAssembly.Version;
                    label1.Text = "SDM Version: " + SDMVersion.ToString();

                    if (!SDMVersion.ToString().Equals(latest.ToString()))
                    {
                        DownloadNewVersion();
                    }
                    else
                    {
                        MessageBox.Show("The current version is up to date.");
                    }
                }
            }
            catch
            {
                MessageBox.Show("There was a problem trying to request the current version. Please check your internet connection and try again. The github repository may be down.");
            }
        }

        private void CheckForCompletion()
        {
            if(Progress >= 2)
            {
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
                {
                    label1.Text = "SDM Version: None";
                    SDMVersion = new Version();

                    DownloadNewVersion();
                    return;
                }
                else
                {
                    AssemblyName SDMAssembly = AssemblyName.GetAssemblyName(Directory.GetCurrentDirectory() + "\\sdmf.exe");
                    SDMVersion = SDMAssembly.Version;
                    label1.Text = "SDM Version: " + SDMVersion.ToString();

                    waiter.Close();
                    if (!File.Exists(Directory.GetCurrentDirectory() + "\\sdmf.exe"))
                    {
                        MessageBox.Show("There is a new version, but the auto-updater failed to download it. Please send an email to tanner@shoutz.com about error code 1. \n\nYou can still download the newest version here: https://github.com/Dytonis/ShoutzDatabaseManager/tree/master/Release", "Error code 1");
                    }
                    else
                    {
                        string version = "";
                        if (OldVersion.ToString() == "0.0")
                        {
                            version = "None";
                        }
                        else
                        {
                            version = OldVersion.ToString();
                        }
                        MessageBox.Show("There was an update!\n\nOld Version: " + version + "\nNew Version: " + SDMVersion.ToString(), "Completed");
                    }
                }
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
