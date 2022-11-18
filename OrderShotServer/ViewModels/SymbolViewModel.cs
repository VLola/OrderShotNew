using Binance.Net.Clients;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Library.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using static OrderShotServer.ViewModels.MainViewModel;

namespace OrderShotServer.ViewModels
{
    public class SymbolViewModel
    {
        private UpdateSubscription _updateSubscriptionToAggregatedTradeUpdates;
        private UpdateSubscription _updateSubscriptionToMiniTickerUpdates;
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        public SymbolModel Symbol { get; set; }
        public ObservableCollection<StrategyViewModel> Strategies { get; set; } = new();
        private BinanceSocketClient _socketClient { get; set; }
        private BinanceClient _client { get; set; }
        public SymbolViewModel(BinanceClient client, BinanceSocketClient socketClient, SymbolSettingsModel symbolSettings, decimal usdt)
        {
            Symbol = new();
            _client = client;
            _socketClient = socketClient;
            Symbol.Name = symbolSettings.Name;
            Symbol.MinQuantity = symbolSettings.MinQuantity;
            Symbol.StepSize = symbolSettings.StepSize;
            Symbol.TickSize = symbolSettings.TickSize;
            Symbol.Usdt = usdt;
            Subscribe();
        }

        public void CloseSymbol()
        {
            UnsubscribeAsync();
        }
        private async void UnsubscribeAsync()
        {
            try
            {
                if (_updateSubscriptionToAggregatedTradeUpdates != null)
                {
                    await _socketClient.UnsubscribeAsync(_updateSubscriptionToAggregatedTradeUpdates);
                }
                if (_updateSubscriptionToMiniTickerUpdates != null)
                {
                    await _socketClient.UnsubscribeAsync(_updateSubscriptionToMiniTickerUpdates);
                }
                
            }
            catch (Exception ex)
            {
                WriteLog($"Failed UnsubscribeAsync: {ex.Message}");
            }
        }
        private void Subscribe()
        {
            SubscribeToAggregatedTradeUpdatesAsync();
            SubscribeToMiniTickerUpdatesAsync();
        }
        public StrategyViewModel AddToStrategies(ClientData clientData)
        {
            StrategyViewModel strategyViewModel = new StrategyViewModel(_client, Symbol, clientData);
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Strategies.Add(strategyViewModel);
            });
            return strategyViewModel;
        }
        public void RemoveFromStrategies(StrategyViewModel strategyViewModel)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Strategies.Remove(strategyViewModel);
            });
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
                    _updateSubscriptionToAggregatedTradeUpdates = result.Data;
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
                    _updateSubscriptionToMiniTickerUpdates = result.Data;
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
                if (!Directory.Exists(_pathLog)) Directory.CreateDirectory(_pathLog);
                File.AppendAllText(_pathLog + Symbol.Name, $"{DateTime.Now} {text}\n");
            }
            catch { }
        }
    }
}
