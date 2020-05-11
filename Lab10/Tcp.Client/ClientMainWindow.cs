using System;
using System.Windows.Forms;
using SomeProject.Library.Client;
using SomeProject.Library;

namespace SomeProject.TcpClient
{
    public partial class ClientMainWindow : Form
    {
        public int id = 0;
        int tim = 10000;
        public ClientMainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Этот метод обрабатывает нажатие кнопки отправки сообщения</summary>
        private void OnMsgBtnClick(object sender, EventArgs e)
        {
            Client client = new Client(id);
            Result res = client.SendMessageToServer(textBox.Text).Result;
            if(res == Result.OK)
            {
                textBox.Text = "";
                labelRes.Text = "Message was sent succefully!";
                label2.Text = client.ReceiveMessageFromServer().Message;
            }
            else
            {
                labelRes.Text = "Cannot send the message to the server.";
                label2.Text = client.ReceiveMessageFromServer().Message;
            }
            timer.Interval = 2000;
            timer.Start();
        }
        /// <summary>
        /// Этот метод убирает надписи с Label через определенное количество времени</summary>
        private void OnTimerTick(object sender, EventArgs e)
        {
            labelRes.Text = "";
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            timer.Stop();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            textBox1.Text = openFileDialog1.FileName;
        }

        /// <summary>
        /// Этот метод обрабатывает нажатие кнопки отправки файла</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            Client client = new Client(id);
            Result res = client.SendFileToServer(textBox1.Text).Result;
            if (res == Result.OK)
            {
                label1.Text = "File was sent succefully!";
                label3.Text = client.ReceiveMessageFromServer().Message;
            }
            else
            {
                label1.Text = "Cannot send the file to the server.";
                label3.Text = client.ReceiveMessageFromServer().Message;
            }
            timer.Interval = 2000;
            timer.Start();
        }
        /// <summary>
        /// Этот метод обрабатывает запуск формы, и отправляет серверу запрос на получение Id</summary>
        private void ClientMainWindow_Load(object sender, EventArgs e)
        {
            Client client = new Client(id);
            Result res = client.SendMessageToServer("").Result;
            timer.Interval = 2000;
            timer.Start();
            id = client.ReceiveIdFromServer();
            if (id != 0)
            {
                labelRes.Text = "User added! Id: " + id.ToString();
            }
            else
            {
                labelRes.Text = "User has not been added";
                timer1.Interval = 10000;
                timer1.Enabled = true;

                timer2.Interval = 1000;
                timer2.Enabled = true;
            }
        }
        /// <summary>
        /// Этот метод обрабатывает закрытие формы, и отправляет серверу запрос на получение исключение его из списка</summary>
        private void ClientMainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (id != 0)
            {
                Client client = new Client(id);
                Result res = client.SendCloseToServer("").Result;
            }
        }
        /// <summary>
        /// Этот метод отвечает за повторное подключение к серверу через определеннное количество времени</summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tim = 10000;
            timer2.Interval = 1000;
            timer2.Enabled = true;
            if (id != 0)
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
            else
            {
                Client client = new Client(id);
                Result res = client.SendMessageToServer("").Result;
                timer.Interval = 2000;
                timer.Start();
                id = client.ReceiveIdFromServer();
                if (id != 0)
                {
                    labelRes.Text = "User added! Id: " + id.ToString();
                }
                else
                {
                    labelRes.Text = "User has not been added";
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label2.Text = "Before reconnecting: " + (tim / 1000).ToString() + " seconds left";
            tim = tim - 1000;
        }
    }
}
