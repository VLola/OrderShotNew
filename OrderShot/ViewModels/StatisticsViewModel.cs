using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using Library.Models;
using System.IO;

namespace OrderShot.ViewModels
{
    public class StatisticsViewModel
    {
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        private List<ClientData> _listSymbols = new();
        public ObservableCollection<StrategyModel> StatisticsModels { get; set; } = new();
        public StatisticsViewModel()
        {
            LoadingAsync();
        }
        private async void LoadingAsync()
        {
            await Task.Run(async() => {
                while (true)
                {
                    await Task.Delay(30000);
                    SelectSymbolsAsync();
                }
            });
        }
        private async void SelectSymbolsAsync()
        {
            await Task.Run(() => {
                try
                {
                    _listSymbols.Clear();
                    if (StatisticsModels.Count > 0)
                    {
                        List<StrategyModel> list = StatisticsModels.Where(symbol => symbol.ShortWin >= 500 || symbol.LongWin >= 500).ToList();
                        if (list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                ClientData data = new ClientData();
                                data.Symbol = item.Name;
                                data.Distance = item.Distance;
                                data.Buffer = item.Buffer;
                                data.TakeProfit = item.TakeProfit;
                                data.StopLoss = item.StopLoss;
                                data.FollowPriceDelay = item.FollowPriceDelay;
                                data.MinQuantity = item.MinQuantity;
                                data.StepSize = item.StepSize;
                                if (item.ShortWin > item.LongWin) data.IsShort = true;
                                else data.IsLong = true;
                                _listSymbols.Add(data);
                            }
                            SendListAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"Filed SelectSymbolsAsync {ex.Message}");
                }
                
            });
        }
        private async void SendListAsync()
        {
            await Task.Run(() => {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8088));
                    socket.Send(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(_listSymbols)));
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception ex)
                {
                    WriteLog($"Filed SendListAsync {ex.Message}");
                }
            });
        }

        private void WriteLog(string text)
        {
            try
            {
                File.AppendAllText(_pathLog + "_STATISTICS_LOG", $"{DateTime.Now} {text}\n");
            }
            catch { }
        }

    }
}
