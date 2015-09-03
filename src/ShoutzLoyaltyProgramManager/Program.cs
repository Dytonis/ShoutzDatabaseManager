using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoutzDatabaseManager.Editors;

namespace ShoutzDatabaseManager
{
    static class Program
    {
        public static Main MainWindow;
        public static System.Data.SqlClient.SqlConnection HADESConnection;
        public static System.Data.SqlClient.SqlConnection CERBERUSConnection;
        public static System.Data.SqlClient.SqlConnection LACHESISConnection;

        public static UserAdmin Administrator;
        public static UsersEditor UsersEditor;
        public static ContestEditor ContestEditor;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow = new ShoutzDatabaseManager.Main();

            try
            {
                Application.Run(new Splash());
                Application.Run(MainWindow);
            }
            catch(Exception er)
            {
                MessageBox.Show("Fatal error:\n\t" + er.Message + "\n\t" + er.StackTrace);
            }
        }
    }
}
