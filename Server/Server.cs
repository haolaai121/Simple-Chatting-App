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

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void ListenButton_Click(object sender, EventArgs e)
        {

            listView1.Items.Add("Server running on 127.0.0.1 9999");
            Thread serverThread = new Thread(Connect);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        IPEndPoint ipepServer;
        Socket listenerSocket;
        List<Socket> clientsockets;

        void close()
        {
            listenerSocket.Close();
        }
        void Connect()
        {
            clientsockets = new List<Socket>();
            listenerSocket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp
            );
            // Gán socket lắng nghe tới địa chỉ IP của máy và port 9999
            ipepServer = new IPEndPoint(IPAddress.Any, 9999);
            listenerSocket.Bind(ipepServer);
            Thread Listen = new Thread(() =>
            {
               try
                {
                    while (true)
                    {
                        listenerSocket.Listen(10);
                        Socket client = listenerSocket.Accept();
                        clientsockets.Add(client);
                        listView1.Items.Add("New client connected ");
                        Thread receive = new Thread(ReceiveData);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    listenerSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );
                    // Gán lại socket lắng nghe tới địa chỉ IP của máy và port 9999
                    ipepServer = new IPEndPoint(IPAddress.Any, 9999);
                }
            });
            Listen.IsBackground = true;
            Listen.Start();
        }

        void Send(Socket client,byte[] data)
        {
            if (client != null)
                client.Send(data);
        }
        void ReceiveData(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024];
                    client.Receive(data);
                    Send(client,data);
                    string text = Encoding.UTF8.GetString(data);
                    listView1.Items.Add(text);
                    foreach (Socket item in clientsockets)
                    {
                        if (item != null && item != client)
                        item.Send(data);                    }
                }
            }
            catch
            {
                clientsockets.Remove(client);
                close();
                MessageBox.Show("Server không thể nhận được tin nhắn");
                
            }
        }
    }
}
