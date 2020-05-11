using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SomeProject.Library.Server
{    
    public class Server
    {
        int numFile = 1;
        TcpListener serverListener;
        int[] ArrCl = {-1,-1};
        int id;
        public Server()
        {
            serverListener = new TcpListener(IPAddress.Loopback, 8081);
        }

        public bool TurnOffListener()
        {
            try
            {
                if (serverListener != null)
                    serverListener.Stop();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn off listener: " + e.Message);
                return false;
            }
        }

        public async Task TurnOnListener()
        {
            try
            {
                if (serverListener != null)
                    serverListener.Start();
                while (true)
                {
                    OperationResult result = await ReceiveFromClient();
                    if (result.Result == Result.Fail)
                        Console.WriteLine("Unexpected error: " + result.Message);
                    else
                    {
                        Console.WriteLine("New message from client: " + result.Message);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot turn on listener: " + e.Message);
            }
        }
        /// <summary>
        /// Этот метод возвращает расширение файла</summary>
        /// <returns>
        /// Расширение файла</returns>
        string Ext(NetworkStream stream)
        {
            int numEx = Convert.ToInt32(stream.ReadByte());
            string ex = "";
            byte[] arrByt = new byte[numEx];
            stream.Read(arrByt, 0, numEx);
            ex = Encoding.UTF8.GetString(arrByt, 0, numEx);
            return ex;
        }
        /// <summary>
        /// Этот метод определяет тип запроса и вызывает необходимые обработчики</summary>
        public async Task<OperationResult> ReceiveFromClient()
        {
            try
            {
                Console.WriteLine("Waiting for connections...");
                TcpClient client = serverListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                OperationResult res;
                if (stream.ReadByte() == 0)
                {
                    if (stream.ReadByte() == 0)
                    {
                        id = -1;
                        for (int i = 0; i <= ArrCl.Length - 1; i++)
                        {
                            if (ArrCl[i] == -1)
                            {
                                id = i + 1;
                                ArrCl[i] = id;
                                break;
                            }
                        }
                        if (id != -1)
                        {
                            SendIdToClient(Convert.ToByte(id));
                            return new OperationResult(Result.OK, "User added. Id: " + id.ToString());
                        }
                        else
                        {
                            SendIdToClient(Convert.ToByte(0));
                            return new OperationResult(Result.OK, "User has not been added");
                        }
                    }
                    else
                    {
                        if (stream.ReadByte() == 0)
                        {
                            res = await ReceiveMessageFromClient(stream);
                        }
                        else
                        {
                            res = await ReceiveFileFromClient(stream);
                        }
                        SendMessageToClient("Server processed request!");
                    }
                }
                else
                {
                    ArrCl[stream.ReadByte() - 1] = -1;
                    return new OperationResult(Result.OK, "User disconnected");
                }
                client.Close();
                stream.Close();
                return res;
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
        }
        /// <summary>
        /// Этот метод обрабатывает сообщения</summary>
        public async Task<OperationResult> ReceiveMessageFromClient(NetworkStream stream)
        {
            StringBuilder recievedMessage = new StringBuilder();

            byte[] data = new byte[256];
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                recievedMessage.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return new OperationResult(Result.OK, recievedMessage.ToString().Substring(1));
        }
        /// <summary>
        /// Этот метод обрабатывает файлы</summary>
        public async Task<OperationResult> ReceiveFileFromClient(NetworkStream stream)
        {
            StringBuilder recievedMessage = new StringBuilder();
            string ex = Ext(stream);
            byte[] data = new byte[256];
            string path = Directory.GetCurrentDirectory() + @"\" + DateTime.Today.ToString("yyyy-MM-dd");
            Directory.CreateDirectory(path);
            FileStream file = new FileStream(path + @"\" + numFile + ex, FileMode.OpenOrCreate);
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                file.Write(data, 0, bytes);
            }
            while (stream.DataAvailable);
            file.Close();
            Interlocked.Increment(ref numFile);
            return new OperationResult(Result.OK, "Save File");
        }
        /// <summary>
        /// Этот метод отправляет сообщения клиенту</summary>
        public OperationResult SendMessageToClient(string message)
        {
            try
            {
                TcpClient client = serverListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
            return new OperationResult(Result.OK, "");
        }
        /// <summary>
        /// Этот метод отправляет Id клиенту</summary>
        public OperationResult SendIdToClient(byte id)
        {
            try
            {
                TcpClient client = serverListener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                stream.WriteByte(id);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                return new OperationResult(Result.Fail, e.Message);
            }
            return new OperationResult(Result.OK, "");
        }
    }
}