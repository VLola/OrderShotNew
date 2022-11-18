using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;
using Library.Models;
using System.IO;

namespace OrderShotServer.ModelViews
{
    public class ServerViewModel
    {
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        public ServerModel ServerModel { get; set; }
        public ServerViewModel()
        {
            ServerModel = new();
            ListenAsync();
        }

        private async void ListenAsync()
        {
            await Task.Run(() => {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8088);
                Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(10);
                while (true)
                {
                    ConnectAsync(serverSocket.Accept());
                }
            });
        }

        private async void ConnectAsync(Socket clientSocket)
        {
            await Task.Run(() => {
                while (true)
                {
                    try
                    {
                        int bytes = 0;
                        byte[] buffer = new byte[100000];
                        StringBuilder builder = new StringBuilder();
                        do
                        {
                            bytes = clientSocket.Receive(buffer);
                        } while (clientSocket.Available > 0);
                        builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));

                        if (builder.ToString() == "")
                        {
                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();
                            break;
                        }
                        else
                        {
                            ServerModel.Symbols = JsonConvert.DeserializeObject<List<ClientData>?>(builder.ToString());
                        }
                    }
                    catch (Exception ex) {
                        WriteLog($"Filed ConnectAsync {ex.Message}");
                    }
                }
            });
        }
        private void WriteLog(string text)
        {
            try
            {
                if (!Directory.Exists(_pathLog)) Directory.CreateDirectory(_pathLog);
                File.AppendAllText($"{_pathLog}_SERVER_LOG", $"{DateTime.Now} {text}\n");
            }
            catch { }
        }
    }
}
