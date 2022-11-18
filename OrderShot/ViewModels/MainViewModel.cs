using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using Library.Command;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderShot.ViewModels
{
    public class MainViewModel
    {
        string ApiKey = "a4c675ddfa8005fdabf5580700bd87b2d0dff9108b1caa8295f5540e6cf118e5";
        string SecretKey = "211c4565fb98ad121a10ce2cce9c31456890786cbce501ad426b0bbace6e1102";
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        public MainModel MainModel { get; set; } = new();
        public LoginModel LoginModel { get; set; } = new();

        public ObservableCollection<SymbolViewModel> Symbols { get; set; } = new();
        public StatisticsViewModel StatisticsViewModel { get; set; } = new();
        public ObservableCollection<StrategyViewModel> ViewedStrategies { get; set; } = new();
        private BinanceClient? _client { get; set; }
        private BinanceSocketClient? _socketClient { get; set; }
        private RelayCommand? _addStrategiesCommand;
        public RelayCommand AddStrategiesCommand
        {
            get { return _addStrategiesCommand ?? (_addStrategiesCommand = new RelayCommand(obj => { AddStrategiesAsync(); })); }
        }
        private RelayCommand? _loginClientCommand;
        public RelayCommand LoginClientCommand
        {
            get { return _loginClientCommand ?? (_loginClientCommand = new RelayCommand(obj => { LoginClient(); })); }
        }
        public MainViewModel()
        {
            if (!Directory.Exists(_pathLog)) Directory.CreateDirectory(_pathLog);
            
        }
        private void LoginClient()
        {
            if (LoginModel.IsTestnet)
            {
                // ------------- Test Api ----------------
                BinanceClientOptions clientOption = new();
                clientOption.UsdFuturesApiOptions.BaseAddress = "https://testnet.binancefuture.com";
                _client = new(clientOption);

                BinanceSocketClientOptions socketClientOption = new BinanceSocketClientOptions
                {
                    AutoReconnect = true,
                    ReconnectInterval = TimeSpan.FromMinutes(1)
                };
                socketClientOption.UsdFuturesStreamsOptions.BaseAddress = "wss://stream.binancefuture.com";
                _socketClient = new BinanceSocketClient(socketClientOption);


                _client.SetApiCredentials(new ApiCredentials(ApiKey, SecretKey));
                _socketClient.SetApiCredentials(new ApiCredentials(ApiKey, SecretKey));
                // ------------- Test Api ----------------
            }
            else
            {
                _client = new();
                _socketClient = new();
            }

            MainModel.PropertyChanged += MainModel_PropertyChanged;
            GetSumbolName();
            LoginModel.IsLogin = true;
        }
        private void MainModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRunChart")
            {
                SetSettingsAsync("IsRunChart");
            }
            else if (e.PropertyName == "MinVolume")
            {
                SetSettingsAsync("MinVolume");
            }
        }
        private async void SetSettingsAsync(string name)
        {
            await Task.Run(() =>
            {
                foreach (var item in Symbols)
                {
                    if (name == "IsRunChart")
                    {
                        foreach (var it in item.Strategies)
                        {
                            it.ChartViewModel.ChartModel.IsRun = MainModel.IsRunChart;
                        }
                    }
                    else if (name == "MinVolume")
                    {
                        item.Symbol.MinVolume = MainModel.MinVolume;
                    }
                }
            });
        }

        public async void AddStrategiesAsync()
        {
            await Task.Run(async () =>
            {
                ClientData clientData = new() { 
                    Distance = MainModel.DistanceSetting, 
                    Buffer = MainModel.BufferSetting, 
                    TakeProfit = MainModel.TakeProfitSetting, 
                    StopLoss = MainModel.StopLossSetting,
                    FollowPriceDelay = MainModel.FollowPriceDelaySetting
                };
                foreach (var item in Symbols)
                {
                    StrategyViewModel strategyViewModel = item.AddStrategy(clientData);
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        StatisticsViewModel.StatisticsModels.Add(strategyViewModel.StrategyModel);
                    });
                }
            });
        }

        private void Strategies_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {

                StrategyViewModel addedItem = (StrategyViewModel)e.NewItems[0];
                addedItem.PropertyChanged += Strategy_PropertyChanged;
            }
        }

        private void Strategy_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsView")
            {
                StrategyViewModel strategyViewModel = (StrategyViewModel)sender;
                if (strategyViewModel.IsView)
                {
                    ViewedStrategies.Add(strategyViewModel);
                }
                else
                {
                    ViewedStrategies.Remove(strategyViewModel);
                }
            }
        }
        private void GetSumbolName()
        {
            //List<string> list = new();
            //foreach (var it in ListSymbols())
            //{
            //    list.Add(it.Symbol);
            //}
            //list.Sort();
            foreach (var it in ExchangeInfo())
            {
                SymbolViewModel symbolViewModel = new(_client, _socketClient, it.Name, it.MinQuantity, it.StepSize);
                symbolViewModel.Strategies.CollectionChanged += Strategies_CollectionChanged;
                Symbols.Add(symbolViewModel);
            }
        }
        private List<Symbol> ExchangeInfo()
        {
            List<Symbol> list = new();
            try
            {
                var result = _client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync().Result;
                if (!result.Success) WriteLog($"Failed ExchangeInfo {result.Error?.Message}");
                else
                {
                    foreach (var it in result.Data.Symbols.ToList())
                    {
                        if (it.Name.Contains("USDT"))
                        {
                            list.Add(new()
                            {
                                Name = it.Name,
                                MinQuantity = it.LotSizeFilter.MinQuantity,
                                StepSize = it.LotSizeFilter.StepSize
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Failed ExchangeInfo {ex.Message}");
            }
            return list;
        }
        //private List<BinancePrice> ListSymbols()
        //{
        //    try
        //    {
        //        var result = _client.UsdFuturesApi.ExchangeData.GetPricesAsync().Result;
        //        if (!result.Success) WriteLog($"Failed Success ListSymbols: {result.Error?.Message}");
        //        return result.Data.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog($"Failed ListSymbols: {ex.Message}");
        //        return null;
        //    }
        //}
        private void WriteLog(string text)
        {
            try
            {
                File.AppendAllText(_pathLog + "_MAIN_LOG", $"{DateTime.Now} {text}\n");
            }
            catch { }
        }
        public class Symbol
        {
            public string Name { get; set; }
            public decimal MinQuantity { get; set; }
            public decimal StepSize { get; set; }
        }
    }
}
