using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonNet;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnSend.Enabled = false;
        }
        private string sHost = "LAPTOP-A8CMBCCM";
        private string usHost = "";
        private Socket cSocket;
        private int port = 8034;
        private NetMessaging net;
        private Thread t = null;
        ThreadStart th;
        private int[,] Matrix;
        string dataMatrix = "";


        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Подключить")
            {
                if (!(usHost.Length > 0))
                {
                    usHost = sHost;
                    txtHost.Text = usHost;
                }
                    
                if (txtUserName.Text.Length > 0)
                {
                    btnConnect.Text = "Отключить";
                    txtUserName.Enabled = false;
                    txtHost.Enabled = false;
                    //проверить имя непустое
                    Connecting();
                }
                else
                {
                    txtUserName.Text = "Введите имя!!!";
                }
                
            }
            else
            {
                Stop();
                btnConnect.Text = "Подключить";
                txtUserName.Enabled = true;
                txtHost.Enabled = true;
                btnSend.Enabled = false;
            }
        }

        private void Connecting()
        {
            try
            {
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(usHost, port);
                net = new NetMessaging(cSocket);
                net.LoginCmdReceived += OnLogin;
                net.StartCmdReceived += OnStart;
                net.MessageCmdReceived += OnMessage;
                net.CheckNameCmdReceived += OnCheckName;
                net.MatrixCmdReceived += OnMatrix;
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }).Start();
            }
            catch (Exception e)
            {
                txtChat.Text = "Что-то пошло не так... :(";
                Stop();
                btnConnect.Text = "Подключить";
                txtUserName.Enabled = true;
                txtHost.Enabled = true;
                btnSend.Enabled = false;
            }
        }


        private void OnMessage(string command, string data)
        {
            //Console.WriteLine("{0}", data);
            //txtChat.AppendText(data + Environment.NewLine);
            //if (!txtChat.InvokeRequired) 
                //txtChat.AppendText(data + Environment.NewLine);
            /*else
            {
                //object[] value = { command, data };
                NetMessaging.SetTextCallback d = new NetMessaging.SetTextCallback(OnMessage);
                Invoke(d, new object[] { command, data });
                //Invoke(new NetMessaging.ReceiveData(OnMessage), value);
            }*/
        }

        private void OnCalculate(string command, string data)
        {
            //NetMessaging.
        }
        private void OnStart(string command, string data)
        {
           
            if (!txtChat.InvokeRequired)
            {
                btnSend.Enabled = true;
                //GoMessaging();
                GoCalculate();
            }
            else
            {
                NetMessaging.SetTextCallback d = new NetMessaging.SetTextCallback(OnStart);
                Invoke(d, new object[] { command, data });
            }
        }


        private string mess ="";
        private void GoCalculate()
        {
            th = new ThreadStart(DataTest);
            t = new Thread(th);
            t.Start();
            new Thread(() =>
            {
                while (true)
                {
                    if (dataMatrix.Length>0)
                    {
                        net.SendData("MATRIX", dataMatrix);
                        dataMatrix = "";
                    }
                    //String userData = "";
                    //userData = Console.ReadLine();
                    //this.Invoke((new NetMessaging(this.cSocket)).))
                    //net.SendData("MESSAGE", userData);
                }
            }
            ).Start();
        }

        private void DataTest()
        {
            while (true)
            {
                if (dataMatrix.Length>0)
                {
                    net.SendData("MATRIX", dataMatrix);
                    dataMatrix = "";
                }
                //String userData = "";
                //userData = Console.ReadLine();
                //this.Invoke((new NetMessaging(this.cSocket)).))
                //net.SendData("MESSAGE", userData);
            }
        }

        void OnLogin(string c, string d)
        {
            String userName = "";
            userName = txtUserName.Text;
            net.SendData("LOGIN", userName);
        }
        private void txtHost_TextChanged(object sender, EventArgs e)
        {
             usHost = txtHost.Text;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Matrix = new int[4,4];
            Matrix[0,0] = Convert.ToInt32(m11.Text);
            Matrix[0,1] = Convert.ToInt32(m12.Text);
            Matrix[0,2] = Convert.ToInt32(m13.Text);
            Matrix[0,3] = Convert.ToInt32(m14.Text);
            Matrix[1,0] = Convert.ToInt32(m21.Text);
            Matrix[1,1] = Convert.ToInt32(m22.Text);
            Matrix[1,2] = Convert.ToInt32(m23.Text);
            Matrix[1,3] = Convert.ToInt32(m24.Text);
            Matrix[2,0] = Convert.ToInt32(m31.Text);
            Matrix[2,1] = Convert.ToInt32(m32.Text);
            Matrix[2,2] = Convert.ToInt32(m33.Text);
            Matrix[2,3] = Convert.ToInt32(m34.Text);
            Matrix[3,0] = Convert.ToInt32(m41.Text);
            Matrix[3,1] = Convert.ToInt32(m42.Text);
            Matrix[3,2] = Convert.ToInt32(m43.Text);
            Matrix[3,3] = Convert.ToInt32(m44.Text);

            for (int i = 0; i<4;i++)
            {
                for(int j = 0; j<4;j++)
                    dataMatrix = dataMatrix + Matrix[i,j] + ' ';
            }
            dataMatrix = dataMatrix + '\n';


        }

       
        void OnCheckName(string c, string d)
        {
            if (d == "?")
                net.SendData("CHECKNAME", txtUserName.Text);
            if (d== "NO")
            {
                if (!txtChat.InvokeRequired)
                {
                    txtChat.Text = "Измените имя и повторите попытку";
                    btnUserName.Visible = true;
                    txtUserName.Enabled = true;

                }
                else
                {
                    NetMessaging.SetTextCallback d1 = new NetMessaging.SetTextCallback(OnCheckName);
                    Invoke(d1, new object[] { "CHECKNAME", "NO" });
                }
            }
            if (d=="YES")
            {
                net.SendData("LOGIN", txtUserName.Text);
            }

        }

        void OnMatrix(string c, string d)
        {
            if (d == "NO")
            {
                //тут надо считаь обратную
                net.SendData("MATRIXANSWER", "полученныйй ответ");
            }
            else
            {
                if (!txtChat.InvokeRequired)
                {
                    txtChat.Text = "Ответ: " + d;
                }
                else
                {
                    NetMessaging.SetTextCallback d1 = new NetMessaging.SetTextCallback(OnMatrix);
                    Invoke(d1, new object[] { "MATRIX", d });
                }
            }
        }

        private void btnUserName_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text.Length > 0)
            {
                net.SendData("CHECKNAME", txtUserName.Text);
                btnUserName.Visible = false;
                txtUserName.Enabled = false;
            }
            else
            {
                txtUserName.Text = "Введите имя!!!";
            }

        }

        private void Stop()
        {
            if (net != null)
                net.SendData("DISCONNECT", "!");
            if (t != null)
            {
                t.Abort();
                t.Join();
                t = null;
            }
            if (cSocket != null)
                cSocket.Close();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();           
        }
    }
}
