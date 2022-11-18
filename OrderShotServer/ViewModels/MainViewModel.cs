using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using Library.Command;
using Library.Models;
using Newtonsoft.Json;
using OrderShotServer.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace OrderShotServer.ViewModels
{
    internal class MainViewModel
    {
        private const string _link = "https://drive.google.com/u/0/uc?id=13RLR9SIMLL2ibwDh8ouByOElk6Yw784J&export=download";
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        private string _pathClients = $"{Directory.GetCurrentDirectory()}/clients/";
        public ObservableCollection<SymbolViewModel> Symbols { get; set; } = new();
        public StatisticsViewModel StatisticsViewModel { get; set; } = new();
        public ObservableCollection<StrategyViewModel> ViewedStrategies { get; set; } = new();
        public ServerViewModel ServerViewModel { get; set; }
        public MainModel MainModel { get; set; } = new();
        public LoginModel LoginModel { get; set; } = new();
        private BinanceClient? _client { get; set; }
        private BinanceSocketClient? _socketClient { get; set; }
        List<SymbolSettingsModel> LIST;
        public delegate void AccountOnOrderUpdate(BinanceFuturesStreamOrderUpdate OrderUpdate);
        public event AccountOnOrderUpdate? OnOrderUpdate;
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
        private RelayCommand? _saveClientCommand;
        public RelayCommand SaveClientCommand
        {
            get { return _saveClientCommand ?? (_saveClientCommand = new RelayCommand(obj => { SaveClient(); })); }
        }
        public MainViewModel()
        {
            if (!Directory.Exists(_pathClients)) Directory.CreateDirectory(_pathClients);
            LoadClients();
        }
        private void LoadClients()
        {
            if (LoginModel.Clients.Count > 0) LoginModel.Clients.Clear();
            foreach (var item in new DirectoryInfo(_pathClients).GetFiles().Select(item => item.Name))
            {
                LoginModel.Clients.Add(item);
            }
            if(LoginModel.Clients.Count > 0) LoginModel.SelectedClient = LoginModel.Clients[0];
        }
        private void LoginClient()
        {
            using (var client = new WebClient())
            {
                string json = client.DownloadString(_link);
                List<ClientModel>? clientModels = JsonConvert.DeserializeObject<List<ClientModel>>(json);
                if (clientModels != null)
                {
                    bool check = false;
                    foreach (var item in clientModels)
                    {
                        if (item.ClientName == "Valentyn") check = item.Access;
                    }
                    if (check)
                    {
                        if(File.Exists(_pathClients + LoginModel.SelectedClient))
                        {
                            ClientLogin clientLogin = JsonConvert.DeserializeObject<ClientLogin>(File.ReadAllText(_pathClients + LoginModel.SelectedClient));
                            Load(clientLogin.ApiKey, clientLogin.SecretKey, LoginModel.IsTestnet);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Login name failed!");
                    }
                }
            }
        }
        private void Load(string apiKey, string secretKey, bool isTestnet)
        {
            if (isTestnet)
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
                // ------------- Test Api ----------------
            }
            else
            {
                // ------------- Real Api ----------------
                _client = new();
                _socketClient = new();
                // ------------- Real Api ----------------
            }

            try
            {
                _client.SetApiCredentials(new ApiCredentials(apiKey, secretKey));
                _socketClient.SetApiCredentials(new ApiCredentials(apiKey, secretKey));

                LoginModel.ApiKey = "";
                LoginModel.SecretKey = "";
                if (CheckLogin())
                {
                    LoginModel.IsLogin = true;
                    LoadMain();
                }
                else MessageBox.Show("Login failed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool CheckLogin()
        {
            try
            {
                var result = _client.UsdFuturesApi.Account.GetAccountInfoAsync().Result;
                if (!result.Success)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private void LoadMain()
        {
            MainModel.PropertyChanged += MainModel_PropertyChanged;
            ServerViewModel = new();
            ServerViewModel.ServerModel.PropertyChanged += ServerModel_PropertyChanged;
            LIST = ExchangeInfo();
            SubscribeToUserDataUpdatesAsync();
        }

        private void MainModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Usdt")
            {
                foreach (var item in Symbols)
                {
                    item.Symbol.Usdt = MainModel.Usdt;
                }
            }
        }

        private void SaveClient()
        {
            if (!Directory.Exists(_pathClients)) Directory.CreateDirectory(_pathClients);
            ClientLogin clientLogin = new() { Name = LoginModel.Name, ApiKey = LoginModel.ApiKey, SecretKey = LoginModel.SecretKey };
            File.WriteAllText(_pathClients + LoginModel.Name, JsonConvert.SerializeObject(clientLogin));
            Clear();
            LoadClients();
        }
        private void Clear()
        {
            LoginModel.Name = "";
            LoginModel.ApiKey = "";
            LoginModel.SecretKey = "";
        }
        private List<SymbolSettingsModel> ExchangeInfo()
        {
            List<SymbolSettingsModel> list = new();
            try
            {
                var result = _client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync().Result;
                if (!result.Success) WriteLog($"Failed ExchangeInfo {result.Error?.Message}");
                else
                {
                    foreach (var it in result.Data.Symbols.ToList())
                    {
                        list.Add(new()
                        {
                            Name = it.Name,
                            MinQuantity = it.LotSizeFilter.MinQuantity,
                            StepSize = it.LotSizeFilter.StepSize,
                            TickSize = it.PriceFilter.TickSize
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Failed ExchangeInfo {ex.Message}");
            }
            return list;
        }
        public async void AddStrategiesAsync()
        {
            await Task.Run(async () =>
            {
                SymbolSettingsModel symbolSettings = LIST.Single(item => item.Name == MainModel.NameSetting); 
                ClientData clientData = new()
                {
                    Symbol = MainModel.NameSetting,
                    IsLong = MainModel.IsLong,
                    IsShort = MainModel.IsShort,
                    Distance = MainModel.DistanceSetting,
                    Buffer = MainModel.BufferSetting,
                    TakeProfit = MainModel.TakeProfitSetting,
                    StopLoss = MainModel.StopLossSetting,
                    FollowPriceDelay = MainModel.FollowPriceDelaySetting
                };
                SymbolViewModel symbolViewModel = new(_client, _socketClient, symbolSettings, MainModel.Usdt);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Symbols.Add(symbolViewModel);
                });
                AddStrategyViewModel(symbolViewModel, clientData);
            });
        }
        private void ServerModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Symbols")
            {
                AddSymbolViewModel();
            }
        }

        private List<ClientData> _symbols { get; set; } = new();
        private void AddSymbolViewModel()
        {
            try
            {
                RemoveStrategies();

                if (ServerViewModel.ServerModel.Symbols != null && ServerViewModel.ServerModel.Symbols.Count > 0)
                {
                    foreach (var item in ServerViewModel.ServerModel.Symbols)
                    {
                        CheckSymbolViewModel(item);
                    }
                }

                _symbols.Clear();

                if (ServerViewModel.ServerModel.Symbols != null && ServerViewModel.ServerModel.Symbols.Count > 0)
                {
                    foreach (var item in ServerViewModel.ServerModel.Symbols)
                    {
                        _symbols.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                WriteLog($"Filed AddSymbolViewModel {ex.Message}");
            }
        }
        private void CheckSymbolViewModel(ClientData clientData)
        {
            try
            {
                if (Symbols.Any(item => item.Symbol.Name == clientData.Symbol)) return;

                SymbolSettingsModel symbolSettings = LIST.Single(item => item.Name == clientData.Symbol);

                SymbolViewModel symbolViewModel = new(_client, _socketClient, symbolSettings, MainModel.Usdt);
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Symbols.Add(symbolViewModel);
                });
                AddStrategyViewModel(symbolViewModel, clientData);
            }
            catch (Exception ex)
            {
                WriteLog($"Filed CheckSymbolViewModel {ex.Message}");
            }
        }
        private void AddStrategyViewModel(SymbolViewModel symbolViewModel, ClientData clientData)
        {
            StrategyViewModel strategyViewModel = symbolViewModel.AddToStrategies(clientData);
            strategyViewModel.PropertyChanged += Strategy_PropertyChanged;
            OnOrderUpdate += strategyViewModel.OrderUpdate;

            // Add to statistics
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                StatisticsViewModel.StatisticsModels.Add(strategyViewModel.StrategyModel);
            });
        }
        private void RemoveStrategies()
        {
            try
            {
                if (_symbols.Count > 0)
                {
                    foreach (var item in _symbols)
                    {
                        if (ServerViewModel.ServerModel.Symbols != null && ServerViewModel.ServerModel.Symbols.Count > 0)
                        {
                            if (!ServerViewModel.ServerModel.Symbols.Contains(item))
                            {
                                SymbolViewModel? symbolViewModel = Symbols.FirstOrDefault(symbol => symbol.Symbol.Name == item.Symbol);
                                if (symbolViewModel != null) RemoveStrategyViewModel(symbolViewModel, item);
                            }
                        }
                        else
                        {
                            SymbolViewModel? symbolViewModel = Symbols.FirstOrDefault(symbol => symbol.Symbol.Name == item.Symbol);
                            if (symbolViewModel != null) RemoveStrategyViewModel(symbolViewModel, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Filed RemoveStrategies {ex.Message}");
            }
        }
        private void RemoveStrategyViewModel(SymbolViewModel symbolViewModel, ClientData clientData)
        {
            try
            {
                StrategyViewModel? strategyViewModel = symbolViewModel.Strategies.FirstOrDefault(iterator => iterator.StrategyModel.Name == clientData.Symbol);
                if (strategyViewModel != null)
                {
                    strategyViewModel.CloseStrategy();
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Filed RemoveStrategyViewModel {ex.Message}");
            }
        }
        private void Strategy_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsView")
            {
                StrategyViewModel strategyViewModel = (StrategyViewModel)sender;
                if (strategyViewModel.IsView)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ViewedStrategies.Add(strategyViewModel);
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        ViewedStrategies.Remove(strategyViewModel);
                    });
                }
            }
            if (e.PropertyName == "IsClose")
            {
                StrategyViewModel strategyViewModel = (StrategyViewModel)sender;
                if (strategyViewModel.IsClose)
                {
                    strategyViewModel.PropertyChanged -= Strategy_PropertyChanged;
                    OnOrderUpdate -= strategyViewModel.OrderUpdate;

                    SymbolViewModel? symbolViewModel = Symbols.FirstOrDefault(symbol => symbol.Symbol.Name == strategyViewModel.StrategyModel.Name);
                    if (symbolViewModel != null) {

                        symbolViewModel.RemoveFromStrategies(strategyViewModel);

                        // Удалить график

                        //App.Current.Dispatcher.Invoke((Action)delegate
                        //{
                        //    ViewedStrategies.Remove(strategyViewModel);
                        //});

                        if (symbolViewModel.Strategies.Count == 0)
                        {
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Symbols.Remove(symbolViewModel);
                            });
                            symbolViewModel.CloseSymbol();
                        }
                    }
                }
            }
        }
        private async void SubscribeToUserDataUpdatesAsync()
        {
            var listenKey = await _client.UsdFuturesApi.Account.StartUserStreamAsync();
            if (!listenKey.Success)
            {
                WriteLog($"Failed to start user stream: listenKey");
            }
            else
            {
                KeepAliveUserStreamAsync(listenKey.Data);
                WriteLog($"Listen Key Created");
                var result = await _socketClient.UsdFuturesStreams.SubscribeToUserDataUpdatesAsync(listenKey: listenKey.Data,
                    onLeverageUpdate => { },
                    onMarginUpdate => { },
                    onAccountUpdate => { },
                    onOrderUpdate =>
                    {
                        OnOrderUpdate?.Invoke(onOrderUpdate.Data);
                    },
                    onListenKeyExpired => { });
                if (!result.Success)
                {
                    WriteLog($"Failed UserDataUpdates: {result.Error?.Message}");
                }
            }
        }
        private async void KeepAliveUserStreamAsync(string listenKey)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    var result = await _client.UsdFuturesApi.Account.KeepAliveUserStreamAsync(listenKey);
                    if (!result.Success) WriteLog($"Failed KeepAliveUserStreamAsync: {result.Error?.Message}");
                    else
                    {
                        WriteLog("Success KeepAliveUserStreamAsync");
                    }
                    await Task.Delay(900000);
                }
            });
        }
        private void WriteLog(string text)
        {
            try
            {
                File.AppendAllText(_pathLog + "_MAIN_LOG", $"{DateTime.Now} {text}\n");
            }
            catch { }
        }
    }
}
