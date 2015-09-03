using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace ShoutzDatabaseManager.Shared
{
    class Services
    {
        private static volatile Waiting waitPage;

        public static class DataServices
        {
            public static void CreateDataServiceMethod(DataFetchArguments Fetch, RunWorkerCompletedEventHandler OnComplete)
            {
                try
                {
                    //error checks
                    if (Fetch.SqlCommandArguments != null)
                    {
                        foreach (object obj in Fetch.SqlCommandArguments)
                        {
                            if ((string)obj == "")
                            {
                                MessageBox.Show("Blank message sent");
                                return;
                            }
                        }
                    }
                    if (Fetch.SqlCommandArgumentNames != null)
                    {
                        foreach (string obj in Fetch.SqlCommandArgumentNames)
                        {
                            if ((string)obj == "")
                            {
                                MessageBox.Show("Blank message name sent");
                                return;
                            }
                        }
                    }
                    BackgroundWorker AsyncWorker = new BackgroundWorker();
                    AsyncWorker.WorkerSupportsCancellation = true;
                    waitPage = new Waiting();
                    waitPage.Show();
                    AsyncWorker.DoWork += Services.DataServices.Worker_DoWork;
                    AsyncWorker.RunWorkerCompleted += OnComplete;
                    AsyncWorker.RunWorkerCompleted += UIWorker_RunWorkerCompleted;
                    AsyncWorker.RunWorkerAsync(Fetch);
                }
                catch(Exception er)
                {
                    MessageBox.Show("There was an error processing this request; the string may be in the incorrect format.");
                    return;
                }
            }
            public static void CreateDataServiceMethod(DataFetchArguments Fetch)
            {
                try
                {
                    //error checks
                    if (Fetch.SqlCommandArguments != null)
                    {
                        foreach (object obj in Fetch.SqlCommandArguments)
                        {
                            if ((string)obj == "")
                            {
                                MessageBox.Show("Blank message sent");
                                return;
                            }
                        }
                    }
                    if (Fetch.SqlCommandArgumentNames != null)
                    {
                        foreach (string obj in Fetch.SqlCommandArgumentNames)
                        {
                            if ((string)obj == "")
                            {
                                MessageBox.Show("Blank message name sent");
                                return;
                            }
                        }
                    }
                    BackgroundWorker AsyncWorker = new BackgroundWorker();
                    AsyncWorker.WorkerSupportsCancellation = true;
                    waitPage = new Waiting();
                    waitPage.Show();
                    AsyncWorker.DoWork += Services.DataServices.Worker_DoWork;
                    AsyncWorker.RunWorkerCompleted += UIWorker_RunWorkerCompleted;
                    AsyncWorker.RunWorkerAsync(Fetch);
                }
                catch (Exception er)
                {
                    MessageBox.Show("There was an error processing this request; the string may be in the incorrect format.");
                    return;
                }
            }

            private static void UIWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                waitPage.Close();
            }

            private static void Worker_DoWork(object sender, DoWorkEventArgs e)
            {
                BackgroundWorker worker = sender as BackgroundWorker;

                e.Result = GetDataAsync(e.Argument as DataFetchArguments);
                
                worker.DoWork -= Worker_DoWork;
            }

            private static DataTable GetDataAsync(string cmdString, SqlConnection connection, string[] argNames, object[] args, CommandType type)
            {
                try
                {
                    DataTable returns = new DataTable();

                    using (SqlCommand cmd = new SqlCommand(cmdString, connection))
                    {
                        int index = 0;
                        foreach (string arg in argNames)
                        {
                            cmd.Parameters.Add(new SqlParameter(arg, args[index]));
                            index++;
                        }
                        cmd.CommandType = type;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(returns);
                    }

                    return returns;
                }

                catch (Exception er)
                {
                    waitPage.Invoke(new Action(() => { waitPage.Close(); }));
                    MessageBox.Show("There was an error processing this request; the string may be in an incorrect format.");
                    return null;
                }
            }
            private static DataTable GetDataAsync(DataFetchArguments arguments)
            {
                try
                {
                    DataTable returns = new DataTable();

                    using (SqlCommand cmd = new SqlCommand(arguments.Command, arguments.Connection))
                    {
                        if (arguments.SqlCommandArgumentNames != null && arguments.SqlCommandArguments != null)
                        {
                            int index = 0;
                            foreach (string arg in arguments.SqlCommandArgumentNames)
                            {
                                cmd.Parameters.Add(new SqlParameter(arg, arguments.SqlCommandArguments[index]));
                                index++;
                            }
                        }
                        cmd.CommandType = arguments.Type;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(returns);
                    }

                    return returns;
                }

                catch (Exception er)
                {
                    waitPage.Invoke(new Action(() => { waitPage.Close(); }));
                    MessageBox.Show("There was an error processing this request; the string may be in an incorrect format.");
                    return null;
                }
            }            

            public class DataFetchArguments
            {
                public string Command;
                public SqlConnection Connection;
                public string[] SqlCommandArgumentNames;
                public object[] SqlCommandArguments;
                public CommandType Type;
            }
        }
    }
}
