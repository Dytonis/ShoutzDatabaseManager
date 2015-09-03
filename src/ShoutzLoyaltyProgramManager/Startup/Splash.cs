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
using System.Threading;

namespace ShoutzLoyaltyProgramManager
{
    public partial class Splash : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerInitial = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer dotTimer = new System.Windows.Forms.Timer();
        float deltaTime = 0;
        const float initialDelay = 2;
        const int animLength = 100;
        const float smooth = 5;
        float key = 0;
        float target = 100;
        public Splash()
        {
            InitializeComponent();
            timer.Tick += Timer_Tick;
            timerInitial.Tick += TimerInitial_Tick;
            timer.Disposed += Timer_Disposed;
            dotTimer.Interval = 400;
            dotTimer.Tick += DotTimer_Tick;
        }

        int dots = 0;
        private void DotTimer_Tick(object sender, EventArgs e)
        {
            switch (dots)
            {
                case 3:
                    label.Text = "Establishing Connection";
                    dots = 0;
                    break;

                case 0: goto case 1;
                case 1: goto case 2;
                case 2:
                    label.Text += ".";
                    dots++;
                    break; 
            }     
        }
        private Label label;
        public SqlConnection connection;
        public SqlConnection connection2;
        public SqlConnection connection3;
        private void Timer_Disposed(object sender, EventArgs e)
        {
            label = new Label();
            label.Location = new Point(110, 235);
            label.Name = "Label";
            label.Font = new Font(FontFamily.GenericMonospace, 20f);
            label.Size = new Size(35, 13);
            label.AutoSize = true;
            label.TabIndex = 0;
            label.ForeColor = Color.Black;
            label.Text = "Establishing Connection";
            Controls.Add(label);

            dotTimer.Enabled = true;

            Thread thread = new Thread(new ThreadStart(() =>
            {
                string connect = "" +
                    "user id=Talus;" +
                    "password=S1lv3rHand;" +
                    "server=olympus-lothub1.cloudapp.net,42337;" +
                    "database=HADES;" +
                    "Connect Timeout=30;";

                connection = new SqlConnection(connect);

                try
                {
                    connection.StateChange += Connection_StateChange;
                    connection.Open();
                }
                catch (Exception er)
                {
                    DialogResult result = MessageBox.Show("There was an error connecting to the database: \n\n" + er.Message);
                    if (result >= 0)
                    {
                        this.Invoke(new Action(() => { Close(); }));
                    }
                }
            }));
            Thread thread2 = new Thread(new ThreadStart(() =>
            {
                string connect = "" +
                    "user id=Talus;" +
                    "password=S1lv3rHand;" +
                    "server=olympus-lothub1.cloudapp.net,42337;" +
                    "database=CERBERUS;" +
                    "Connect Timeout=30;";

                connection2 = new SqlConnection(connect);

                try
                {
                    connection2.StateChange += Connection_StateChange;
                    connection2.Open();
                }
                catch (Exception er)
                {
                    DialogResult result = MessageBox.Show("There was an error connecting to the database: \n\n" + er.Message);
                    if (result >= 0)
                    {
                        this.Invoke(new Action(() => { Close(); }));
                    }
                }
            }));
            Thread thread3 = new Thread(new ThreadStart(() =>
            {
                string connect = "" +
                    "user id=Talus;" +
                    "password=S1lv3rHand;" +
                    "server=olympus-lothub1.cloudapp.net,42337;" +
                    "database=LACHESIS;" +
                    "Connect Timeout=30;";

                connection3 = new SqlConnection(connect);

                try
                {
                    connection3.StateChange += Connection_StateChange;
                    connection3.Open();
                }
                catch (Exception er)
                {
                    DialogResult result = MessageBox.Show("There was an error connecting to the database: \n\n" + er.Message);
                    if(result >= 0)
                    {
                        this.Invoke(new Action(() => { Close(); }));
                    }
                }
            }));

            thread3.Start();
            thread2.Start();
            thread.Start();
        }

        private volatile int count = 0;
        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Open)
            {
                count++;
                if (count == 3)
                {
                    EnterPasswordWindow pass = new EnterPasswordWindow(this);
                    this.Invoke(new Action(() =>
                    {
                        pass.Show();
                    }));
                }
            }
            else
            {
                MessageBox.Show("Connection state is ", "NOT OK" + e.CurrentState);
            }
        }

        public void PasswordReturn(Shared.Enums.PasswordState state)
        {
            if(state == Shared.Enums.PasswordState.Accepted)
            {
                Program.MainWindow.Authenticated = true;
                Program.HADESConnection = connection;
                Program.CERBERUSConnection = connection2;
                Program.LACHESISConnection = connection3;
                Close(); //closing will run main
            }
            else if (state == Shared.Enums.PasswordState.Denied)
            {
                DialogResult result = MessageBox.Show("Password is incorrect.", "Denied", MessageBoxButtons.RetryCancel);
                if(result == DialogResult.Cancel)
                {
                    Close();
                }
            }
            else if (state == Shared.Enums.PasswordState.Canceled)
            {
                Close();
            }

        }

        DateTime startTime;
        private void TimerInitial_Tick(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            oldTime = newTime = startTime;
            timer.Enabled = true;
            timerInitial.Enabled = false;
            timerInitial.Dispose();
            crP = this.Location;
            newTarget = crP.Y - (key / 2);
        }

        DateTime oldTime;
        Point crP;
        float newTarget;
        DateTime newTime;
        int framesLeft = animLength;
        private void Timer_Tick(object sender, EventArgs e)
        {
            newTime = DateTime.Now;
            TimeSpan diff = newTime.Subtract(oldTime);
            framesLeft -= (int)(diff.TotalMilliseconds / 10);
            oldTime = DateTime.Now;
            float interprolateCurrent = interprolate(key, target, 1 - ((float)framesLeft / (float)animLength), InterprolationStyle.Sqrt);

            Size cr = Size;

            Size = new Size(cr.Width, (int)interprolateCurrent);

            if (framesLeft <= 0)
            {
                timer.Enabled = false;
                timer.Dispose();
            }
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            key = Size.Height;
            target += key;
            timer.Interval = 20;
            timerInitial.Interval = (int)initialDelay * 1000;
            timerInitial.Enabled = true;
        }

        public enum InterprolationStyle
        {
            Linear,
            Sin,
            Sqrt,
            Cos,
            Sinh
        }
        public float interprolate(float first, float second, float interprolation, InterprolationStyle style = InterprolationStyle.Linear)
        {
            switch(style)
            {
                case InterprolationStyle.Linear:
                    return ((second - first) * interprolation) + first;

                case InterprolationStyle.Sin:
                    return ((second - first * (float)Math.Sin(interprolation)) * interprolation) + first;

                case InterprolationStyle.Sqrt:
                    return ((second - first * (float)Math.Sqrt(interprolation)) * interprolation) + first;

                case InterprolationStyle.Cos:
                    return ((second - first * (float)Math.Cos(interprolation)) * interprolation) + first;

                case InterprolationStyle.Sinh:
                    return ((second - first * (float)Math.Sinh(interprolation)) * interprolation) + first;
            }

            throw new ArgumentException("IntprolationStyle \"style\" does not match any known InterprolationStyle.");           
        }
    }
}
