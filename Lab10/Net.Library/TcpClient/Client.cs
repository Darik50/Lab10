using System;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace SomeProject.Library.Client
{
    public class Client
    {
        public int id = 0;
        public TcpClient tcpClient;
        /// <summary>
        /// Этот конструктор определяет Id клиента</summary>
        public Client(int idCl)
        {
            id = idCl;
        }
        /// <summary>
        /// Этот метод ловит сообщения от сервера</summary>
        public OperationResult ReceiveMessageFromServer()
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8081);
                StringBuilder recievedMessage = new StringBuilder();
                byte[] data = new byte[256];
                NetworkStream stream = tcpClient.GetStream();

                do
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    recievedMessage.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);
                stream.Close();
                tcpClient.Close();

                return new OperationResult(Result.OK, recievedMessage.ToString());
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.ToString());
            }
        }
        /// <summary>
        /// Этот метод ловит Id от сервера</summary>
        public int ReceiveIdFromServer()
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8081);
                StringBuilder recievedMessage = new StringBuilder();
                byte[] data = new byte[256];
                NetworkStream stream = tcpClient.GetStream();
                int bytes = stream.ReadByte();
                stream.Close();
                tcpClient.Close();

                return bytes;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        /// <summary>
        /// Этот метод отправляет сообщения серверу</summary>
        public OperationResult SendMessageToServer(string message)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8081);
                NetworkStream stream = tcpClient.GetStream();
                byte[] data = System.Text.Encoding.UTF8.GetBytes("0" + message);
                stream.WriteByte(Convert.ToByte(0));
                stream.WriteByte(Convert.ToByte(id));
                stream.WriteByte(0);
                stream.Write(data, 0, data.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "") ;
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }
        /// <summary>
        /// Этот метод сообщает серверу о закрытии формы клиента</summary>
        public OperationResult SendCloseToServer(string message)
        {
            try
            {
                tcpClient = new TcpClient("127.0.0.1", 8081);
                NetworkStream stream = tcpClient.GetStream();
                byte[] data = System.Text.Encoding.UTF8.GetBytes("0" + message);
                stream.WriteByte(Convert.ToByte(1));
                stream.WriteByte(Convert.ToByte(id));
                stream.WriteByte(0);
                stream.Write(data, 0, data.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }
        /// <summary>
        /// Этот метод отправляет файлу серверу</summary>
        public OperationResult SendFileToServer(string filePath)
        {
            try
            {                
                tcpClient = new TcpClient("127.0.0.1", 8081);
                NetworkStream stream = tcpClient.GetStream();
                List<byte> Arr = new List<byte>();
                Arr.AddRange(System.Text.Encoding.UTF8.GetBytes(Path.GetExtension(filePath)));
                Arr.AddRange(File.ReadAllBytes(filePath));
                byte[] data = Arr.ToArray();
                stream.WriteByte(Convert.ToByte(0));
                stream.WriteByte(Convert.ToByte(id));
                stream.WriteByte(1);
                stream.WriteByte(Convert.ToByte(System.Text.Encoding.UTF8.GetBytes(Path.GetExtension(filePath)).Length));
                stream.Write(data, 0, data.Length);
                stream.Close();
                tcpClient.Close();
                return new OperationResult(Result.OK, "");
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }
    }
}
