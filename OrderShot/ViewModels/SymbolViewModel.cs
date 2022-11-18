using Binance.Net.Clients;
using Library.Command;
using Library.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace OrderShot.ViewModels
{
    public class SymbolViewModel
    {
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        public SymbolModel Symbol { get; set; }
        public ObservableCollection<StrategyViewModel> Strategies { get; set; } = new();
        private BinanceSocketClient _socketClient { get; set; }
        private BinanceClient _client { get; set; }
        private RelayCommand? _addStrategyCommand;
        public RelayCommand AddStrategyCommand
        {
            get { return _addStrategyCommand ?? (_addStrategyCommand = new RelayCommand(obj => {
                ClientData clientData = new() { 
                    Distance = Symbol.DistanceSetting,
                    Buffer = Symbol.BufferSetting,
                    TakeProfit = Symbol.TakeProfitSetting,
                    StopLoss = Symbol.StopLossSetting,
                    FollowPriceDelay = Symbol.FollowPriceDelaySetting
                };
                AddStrategy(clientData);
            })); }
        }
        public SymbolViewModel(BinanceClient client, BinanceSocketClient socketClient, string symbolName, decimal minQuantity, decimal stepSize)
        {
            Symbol = new();
            _client = client;
            _socketClient = socketClient;
            Symbol.Name = symbolName;
            Symbol.MinQuantity = minQuantity;
            Symbol.StepSize = stepSize;
            SubscribeToAggregatedTradeUpdatesAsync();
            SubscribeToMiniTickerUpdatesAsync();
        }

        public StrategyViewModel AddStrategy(ClientData clientData)
        {
            StrategyViewModel strategyViewModel = new StrategyViewModel(Symbol, clientData);
            strategyViewModel.PropertyChanged += StrategyViewModel_PropertyChanged;
            AddToStrategies(strategyViewModel);
            return strategyViewModel;
        }
        private void AddToStrategies(StrategyViewModel strategyViewModel)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Strategies.Add(strategyViewModel);
            });
        }

        private void StrategyViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsClose")
            {
                StrategyViewModel? strategyViewModel = sender as StrategyViewModel;
                if (strategyViewModel != null)
                {
                    Strategies.Remove(strategyViewModel);
                    strategyViewModel.Dispose();
                }
            }
        }

        private async void SubscribeToAggregatedTradeUpdatesAsync()
        {
            try
            {
                var result = await _socketClient.UsdFuturesStreams.SubscribeToAggregatedTradeUpdatesAsync(Symbol.Name, Message =>
                {
                    Symbol.BuyerIsMaker = Message.Data.BuyerIsMaker;
                    Symbol.TimeDouble = Message.Data.TradeTime.ToOADate();
                    Symbol.PriceDouble = Decimal.ToDouble(Message.Data.Price);
                    Symbol.Time = Message.Data.TradeTime;
                    Symbol.Price = Message.Data.Price;
                });
                if (!result.Success) WriteLog($"Failed Success SubscribeToAggregatedTradeUpdatesAsync: {result.Error?.Message}");
                else
                {
                    //_updateSubscription = result.Data;
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Failed SubscribeToAggregatedTradeUpdatesAsync: {ex.Message}");
            }
        }
        private async void SubscribeToMiniTickerUpdatesAsync()
        {
            try
            {
                var result = await _socketClient.UsdFuturesStreams.SubscribeToMiniTickerUpdatesAsync(Symbol.Name, Message =>
                {
                    Symbol.Volume = Message.Data.Volume * ((Message.Data.LowPrice + Message.Data.HighPrice) / 2);
                });
                if (!result.Success) WriteLog($"Failed Success SubscribeToAggregatedTradeUpdatesAsync: {result.Error?.Message}");
                else
                {
                    //_updateSubscription = result.Data;
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Failed SubscribeToAggregatedTradeUpdatesAsync: {ex.Message}");
            }
        }
        private void WriteLog(string text)
        {
            try
            {
                File.AppendAllText(_pathLog + Symbol.Name, $"{DateTime.Now} {text}\n");
            }
            catch { }
        }
    }
}
