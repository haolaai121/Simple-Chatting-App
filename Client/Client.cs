using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //các giá trị cần thiết
        IPEndPoint ipepClient;
        TcpClient Tcpclient;
        NetworkStream ns;

        //kết nối tới server
        void Connect()
        {
            Tcpclient = new TcpClient();

            ipepClient = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            try
            {
                Tcpclient.Connect(ipepClient);
                ns = Tcpclient.GetStream();
            } catch
            {
                MessageBox.Show("Không thể kết nối tới server!");
                return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }
        //đóng kết nối
        void Closeclient()
        {
            ns.Close();
            Tcpclient.Close();
        }
        //gửi tin
        void Send()
        {
            if (Message.Text != string.Empty && NameofUser.Text != string.Empty)
            {
                string temp = NameofUser.Text + ": " + Message.Text;
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(temp);
                ns.Write(data, 0, data.Length);
            }
            else
                MessageBox.Show("Vui lòng nhập đủ tên và nội dung muốn gửi !");
           

        }
        //nhận tin
        void Receive()
        {
            try
            {
                while (Tcpclient.Connected)
                {
                    byte[] data = new byte[1024];
                    ns.Read(data, 0, 1024);
                    string text = Encoding.UTF8.GetString(data);
                    listView1.Items.Add(text);
                }
            } catch
            {
                Closeclient();
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Thread SendingMessage = new Thread(Send);
            SendingMessage.IsBackground = true;
            SendingMessage.Start();
        }

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Closeclient();
        }
    }
}
