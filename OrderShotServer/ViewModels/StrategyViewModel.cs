using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Futures.Socket;
using CryptoExchange.Net.CommonObjects;
using Library.Command;
using Library.Models;
using Newtonsoft.Json;
using OrderShotControlLibrary.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OrderShotServer.ViewModels
{
    public class StrategyViewModel : INotifyPropertyChanged
    {
        private string _pathLog = $"{Directory.GetCurrentDirectory()}/log/";
        public void CloseStrategy()
        {
            if (!IsWait)
            {
                WriteLog("CloseStrategy");
                IsWait = true;
                StopStrategy();
            }
        }
        private async void StopStrategy()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (!StrategyModel.IsOpenOrder)
                    {
                        StrategyModel.IsStop = true;
                        break;
                    }
                }
                Unsubscribe();
                ChartViewModel.Unsubscribe();
                ChartViewModel.ChartModel.IsClose = true;
                await Task.Delay(3000);
                ClearOrdersToSymbol();
                WriteLog("StopStrategy");
                IsClose = true;
            });
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private bool _isView { get; set; } = false;
        public bool IsView
        {
            get { return _isView; }
            set
            {
                _isView = value;
                OnPropertyChanged("IsView");
            }
        }
        private bool _isClose { get; set; } = false;
        public bool IsClose
        {
            get { return _isClose; }
            set
            {
                _isClose = value;
                OnPropertyChanged("IsClose");
            }
        }
        private bool _isWait { get; set; } = false;
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                _isWait = value;
                OnPropertyChanged("IsWait");
            }
        }
        private RelayCommand? _visibleStrategyCommand;
        public RelayCommand VisibleStrategyCommand
        {
            get { return _visibleStrategyCommand ?? (_visibleStrategyCommand = new RelayCommand(obj => { VisibleStrategy(); })); }
        }
        private RelayCommand? _closeStrategyCommand;
        public RelayCommand CloseStrategyCommand
        {
            get { return _closeStrategyCommand ?? (_closeStrategyCommand = new RelayCommand(obj => { CloseStrategy(); })); }
        }
        private RelayCommand? _hiddenStrategyCommand;
        public RelayCommand HiddenStrategyCommand
        {
            get { return _hiddenStrategyCommand ?? (_hiddenStrategyCommand = new RelayCommand(obj => { HiddenStrategy(); })); }
        }
        public TransactionModel TransactionModel { get; set; }
        public StrategyModel StrategyModel { get; set; }
        public ChartViewModel ChartViewModel { get; set; }
        public SymbolModel SymbolModel { get; set; }
        private BinanceClient _client { get; set; }
        public StrategyViewModel(BinanceClient client, SymbolModel symbolModel, ClientData clientData)
        {
            _client = client;
            StrategyModel = new();
            SymbolModel = symbolModel;
            StarStrategy(clientData);
        }
        private async void StarStrategy(ClientData clientData)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (SymbolModel.PriceDouble > 0)
                    {
                        StrategyModel.Name = clientData.Symbol;
                        StrategyModel.Distance = clientData.Distance;
                        StrategyModel.Buffer = clientData.Buffer;
                        StrategyModel.TakeProfit = clientData.TakeProfit;
                        StrategyModel.StopLoss = clientData.StopLoss;
                        StrategyModel.FollowPriceDelay = clientData.FollowPriceDelay;
                        StrategyModel.IsShort = clientData.IsShort;
                        StrategyModel.IsLong = clientData.IsLong;
                        Subscription();
                        SetBufferAndDistance(SymbolModel.PriceDouble);
                        ChartViewModel = new(StrategyModel, SymbolModel);
                        IsView = true;
                        break;
                    }
                }
            });
        }
        public void Subscription()
        {
            SymbolModel.PropertyChanged += Symbol_PropertyChanged;
            StrategyModel.PropertyChanged += StrategyModel_PropertyChanged;
        }
        public void Unsubscribe()
        {
            SymbolModel.PropertyChanged -= Symbol_PropertyChanged;
            //StrategyModel.PropertyChanged -= StrategyModel_PropertyChanged;
        }
        private void VisibleStrategy()
        {
            if(!IsView) IsView = true;
        }

        private void HiddenStrategy()
        {
            if (IsView) IsView = false;
        }

        private void StrategyModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsView")
            {
                VisibleStrategy();
            }
            else if (e.PropertyName == "IsHistoryView")
            {
                TransactionHistoryView transactionHistoryView = new(StrategyModel);
                transactionHistoryView.Show();
            }
        }
        private void TransactionModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsView")
            {
                TransactionModel? transactionModel = sender as TransactionModel;
                if (transactionModel != null)
                {
                    TransactionView transactionView = new(StrategyModel.HistoryModel.Get(transactionModel.OpenTime, transactionModel.CloseTime));
                    transactionView.Show();
                }
            }
        }

        private void Symbol_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Price" && !StrategyModel.IsStop)
            {
                if (SymbolModel.BuyerIsMaker)
                {
                    StrategyModel.HistoryModel.PointsIsMaker.Add((SymbolModel.TimeDouble, SymbolModel.PriceDouble));
                }
                else
                {
                    StrategyModel.HistoryModel.PointsIsBuyer.Add((SymbolModel.TimeDouble, SymbolModel.PriceDouble));
                }
                if (StrategyModel.IsOpenOrder)
                {
                    if (StrategyModel.IsLong)
                    {
                        if (SymbolModel.PriceDouble < StrategyModel.StopLossOpenOrder)
                        {
                            CancelAllOrdersAsync();
                            OpenOrderMarketAsync(OrderSide.Sell, Quantity);
                            CloseOrder();
                        }
                    }
                    else if (StrategyModel.IsShort)
                    {
                        if (SymbolModel.PriceDouble > StrategyModel.StopLossOpenOrder)
                        {
                            CancelAllOrdersAsync();
                            OpenOrderMarketAsync(OrderSide.Buy, Quantity);
                            CloseOrder();
                        }
                    }
                }
                if (!StrategyModel.IsOpenOrder)
                {
                    if (SymbolModel.PriceDouble > StrategyModel.UpperBuffer)
                    {

                        if (!StrategyModel.IsFollowPriceDelay)
                        {
                            StrategyModel.IsFollowPriceDelay = true;
                            CheckFollowPriceDelayAsync();
                        }
                    }
                    else if (SymbolModel.PriceDouble < StrategyModel.LowerBuffer)
                    {
                        if (!StrategyModel.IsFollowPriceDelay)
                        {
                            StrategyModel.IsFollowPriceDelay = true;
                            CheckFollowPriceDelayAsync();
                        }
                    }
                }
                // Updates lines to history
                StrategyModel.HistoryModel.UpdateTimeLine();
            }
            
        }
        private void ClearOrdersToSymbol()
        {
            CancelAllOrdersAsync();
            GetPositionInformationAsync();
        }
        private async void CancelAllOrdersAsync()
        {
            var result = await _client.UsdFuturesApi.Trading.CancelAllOrdersAsync(symbol: StrategyModel.Name);
            if (!result.Success)
            {
                WriteLog($"Failed CancelAllOrdersAsync: {result.Error?.Message}");
            }
            else
            {
                WriteLog("CancelAllOrdersAsync");
            }
        }
        private async void GetPositionInformationAsync()
        {
            var result = await _client.UsdFuturesApi.Account.GetPositionInformationAsync(symbol: StrategyModel.Name);
            if (!result.Success)
            {
                WriteLog($"Failed GetPositionInformationAsync: {result.Error?.Message}");
            }
            else
            {
                WriteLog("GetPositionInformationAsync:");
                decimal quantity = result.Data.ToList()[0].Quantity;
                if (quantity != 0m)
                {
                    if (quantity > 0m)
                    {
                        OpenOrderMarketAsync(OrderSide.Sell, quantity);
                    }
                    else
                    {
                        OpenOrderMarketAsync(OrderSide.Buy, -quantity);
                    }
                }
            }
        }
        private async void OpenOrderMarketAsync(OrderSide side, decimal quantity)
        {
            try
            {
                await Task.Run(async() =>
                {
                    WriteLog("Open market order");
                    var result = await _client.UsdFuturesApi.Trading.PlaceOrderAsync(
                        symbol: StrategyModel.Name,
                        side: side,
                        type: FuturesOrderType.Market,
                        quantity: quantity,
                        positionSide: PositionSide.Both);
                    if (!result.Success)
                    {
                        WriteLog($"Failed OpenOrderMarketAsync: {result.Error?.Message}");
                        //await Task.Delay(1000);
                        //OpenOrderMarketAsync(side, quantity);
                    }
                });
            }
            catch (Exception ex)
            {
                WriteLog($"Exception OpenOrderMarketAsync: {ex.Message}");
            }
        }

        #region - Take profit order -
        long TakeProfitOrderId = 0;
        private async Task<long> OpenOrderTakeProfitAsync(OrderSide side, decimal price, decimal quantity)
        {
            WriteLog("Open take profit order");
            var result = await _client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol: StrategyModel.Name,
                    side: side,
                    type: FuturesOrderType.Limit,
                    price: price,
                    quantity: quantity,
                    positionSide: PositionSide.Both,
                    timeInForce: TimeInForce.GoodTillCanceled);
            if (!result.Success) {
                WriteLog($"Failed OpenOrderLimitAsync: {result.Error?.Message}");
                return 0;
            }
            else
            {
                return result.Data.Id;
            }
        }

        #endregion

        private double RoundPriceDouble(double price)
        {
            return Math.Round(price, SymbolModel.RoundPrice);
        }
        private decimal RoundPriceDecimal(decimal price)
        {
            return Math.Round(price, SymbolModel.RoundPrice);
        }
        private decimal RoundQuantity(decimal quantity)
        {
            decimal quantity_final = 0m;
            if (SymbolModel.StepSize == 0.001m) quantity_final = Math.Round(quantity, 3);
            else if (SymbolModel.StepSize == 0.01m) quantity_final = Math.Round(quantity, 2);
            else if (SymbolModel.StepSize == 0.1m) quantity_final = Math.Round(quantity, 1);
            else if (SymbolModel.StepSize == 1m) quantity_final = Math.Round(quantity, 0);
            if(quantity_final < SymbolModel.MinQuantity) return SymbolModel.MinQuantity;
            return quantity_final;
        }

        #region - Open limit order (Set Buffer and Distance) -

        long LimitOrderId = 0;
        decimal Quantity = 0m;
        decimal Commission = 0m;

        private void SetBufferAndDistance(double price)
        {
            if (!StrategyModel.IsOpenOrder && !StrategyModel.IsWaitRestartDalay && !StrategyModel.IsStop)
            {
                if (LimitOrderId != 0)
                {
                    if (CancelLimitOrderAsync(LimitOrderId).Result)
                    {
                        LimitOrderId = 0;
                    }
                    else
                    {
                        LimitOrderId = -1;
                    }
                }
                if(LimitOrderId == 0)
                {
                    StrategyModel.UpperBuffer = price + (price * StrategyModel.Buffer / 200);
                    StrategyModel.LowerBuffer = price - (price * StrategyModel.Buffer / 200);

                    double upperDistance = price + (price * StrategyModel.Distance / 100);
                    double lowerDistance = price - (price * StrategyModel.Distance / 100);


                    decimal openQuantity = RoundQuantity(SymbolModel.Usdt / SymbolModel.Price);

                    if (openQuantity * SymbolModel.Price < 10.5m)
                    {
                        openQuantity += SymbolModel.StepSize;
                    }
                    StrategyModel.Quantity = openQuantity;

                    StrategyModel.LowerDistance = RoundPriceDouble(lowerDistance);
                    StrategyModel.UpperDistance = RoundPriceDouble(upperDistance);

                    if (StrategyModel.IsLong)
                    {
                        LimitOrderId = OpenOrderLimitAsync(OrderSide.Buy, Convert.ToDecimal(StrategyModel.LowerDistance), StrategyModel.Quantity).Result;
                        Quantity = 0m;
                        Commission = 0m;
                    }
                    if (StrategyModel.IsShort)
                    {
                        LimitOrderId = OpenOrderLimitAsync(OrderSide.Sell, Convert.ToDecimal(StrategyModel.UpperDistance), StrategyModel.Quantity).Result;
                        Quantity = 0m;
                        Commission = 0m;
                    }

                    // Add lines to history
                    StrategyModel.HistoryModel.AddLines(StrategyModel.UpperBuffer, StrategyModel.LowerBuffer, StrategyModel.UpperDistance, StrategyModel.LowerDistance);

                }
            }
        }
        private void SetBufferAndDistanceToChart()
        {
            if (!StrategyModel.IsOpenOrder) StrategyModel.IsMovingBufferAndDistance = true;
        }
        private async Task<bool> CancelLimitOrderAsync(long orderId)
        {
            var result = await _client.UsdFuturesApi.Trading.CancelOrderAsync(symbol: StrategyModel.Name, orderId: orderId);
            if (!result.Success)
            {
                WriteLog($"Failed CancelOrderAsync: {result.Error?.Message}");
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<long> OpenOrderLimitAsync(OrderSide side, decimal price, decimal quantity)
        {
            var result = await _client.UsdFuturesApi.Trading.PlaceOrderAsync(
                    symbol: StrategyModel.Name,
                    side: side,
                    type: FuturesOrderType.Limit,
                    price: price,
                    quantity: quantity,
                    positionSide: PositionSide.Both,
                    timeInForce: TimeInForce.GoodTillCanceled);
            if (!result.Success)
            {
                WriteLog($"Failed OpenOrderLimitAsync: {result.Error?.Message}");
                return 0;
            }
            else
            {
                return result.Data.Id;
            }
        }
        private async Task<decimal> OrderInfoAsync(long orderId)
        {
            var result = await _client.UsdFuturesApi.Trading.GetOrderAsync(StrategyModel.Name, orderId);
            if (!result.Success)
            {
                WriteLog($"Failed OrderInfoAsync: {result.Error?.Message}");
                return 0m;
            }
            else
            {
                return result.Data.Quantity;
            }
        }

        private void AddTransaction()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                TransactionModel.PropertyChanged += TransactionModel_PropertyChanged;
                StrategyModel.TransactionHistory.Add(TransactionModel);
            });
        }

        #endregion

        public void OrderUpdate(BinanceFuturesStreamOrderUpdate OrderUpdate)
        {
            if (OrderUpdate.UpdateData.Symbol == SymbolModel.Name && !StrategyModel.IsStop)
            {
                if (OrderUpdate.UpdateData.Status == OrderStatus.Expired)
                {
                    if(OrderUpdate.UpdateData.Type == FuturesOrderType.Market)
                    {
                        OpenOrderMarketAsync(OrderUpdate.UpdateData.Side, OrderUpdate.UpdateData.Quantity);
                    }
                }
                else if (OrderUpdate.UpdateData.Status == OrderStatus.PartiallyFilled)
                {
                    if (OrderUpdate.UpdateData.Type == FuturesOrderType.Limit)
                    {
                        if (StrategyModel.IsLong && OrderUpdate.UpdateData.Side == OrderSide.Buy || StrategyModel.IsShort && OrderUpdate.UpdateData.Side == OrderSide.Sell)
                        {
                            StrategyModel.IsOpenOrder = true;
                            CancelAllOrdersAsync();
                            Quantity += OrderUpdate.UpdateData.QuantityOfLastFilledTrade;
                            Commission += OrderUpdate.UpdateData.Fee;
                            if (!StrategyModel.IsPartiallyFilled)
                            {
                                StrategyModel.IsPartiallyFilled = true;
                                PartiallyFilledOpenOrder(OrderUpdate.UpdateData.OrderId, OrderUpdate.UpdateData.AveragePrice, OrderUpdate.UpdateData.Side, OrderUpdate.UpdateData.UpdateTime);
                            }
                        }
                    }
                    else if (OrderUpdate.UpdateData.Type == FuturesOrderType.Market)
                    {
                        if (StrategyModel.IsLong && OrderUpdate.UpdateData.Side == OrderSide.Sell || StrategyModel.IsShort && OrderUpdate.UpdateData.Side == OrderSide.Buy)
                        {
                            TransactionModel.Commission += OrderUpdate.UpdateData.Fee;
                            TransactionModel.Profit += OrderUpdate.UpdateData.RealizedProfit;
                        }
                    }
                }
                else if (OrderUpdate.UpdateData.Status == OrderStatus.Filled)
                {
                    if(OrderUpdate.UpdateData.Type == FuturesOrderType.Market)
                    {
                        if (StrategyModel.IsLong && OrderUpdate.UpdateData.Side == OrderSide.Sell || StrategyModel.IsShort && OrderUpdate.UpdateData.Side == OrderSide.Buy)
                        {
                            StrategyModel.IsPositiveBet = false;
                            StrategyModel.PriceCloseOrder = Decimal.ToDouble(OrderUpdate.UpdateData.AveragePrice);
                            // Add to history
                            StrategyModel.HistoryModel.PointsIsCloseNegative.Add((OrderUpdate.UpdateData.UpdateTime.ToOADate(), StrategyModel.PriceCloseOrder));

                            TransactionModel.ClosePrice = OrderUpdate.UpdateData.AveragePrice;
                            TransactionModel.CloseTime = OrderUpdate.UpdateData.UpdateTime;
                            TransactionModel.Commission += OrderUpdate.UpdateData.Fee;
                            TransactionModel.Profit += OrderUpdate.UpdateData.RealizedProfit;
                            AddTransaction();


                            if (StrategyModel.IsLong)
                            {
                                StrategyModel.LongMinus += 1;
                            }
                            else if (StrategyModel.IsShort)
                            {
                                StrategyModel.ShortMinus += 1;
                            }
                        }
                    }
                    else if (OrderUpdate.UpdateData.Type == FuturesOrderType.Limit)
                    {
                        if (StrategyModel.IsLong && OrderUpdate.UpdateData.Side == OrderSide.Sell || StrategyModel.IsShort && OrderUpdate.UpdateData.Side == OrderSide.Buy)
                        {
                            TakeProfitOrderId = 0;
                            StrategyModel.IsPositiveBet = true;
                            StrategyModel.PriceCloseOrder = Decimal.ToDouble(OrderUpdate.UpdateData.AveragePrice);
                            StrategyModel.HistoryModel.PointsIsClosePositive.Add((OrderUpdate.UpdateData.UpdateTime.ToOADate(), StrategyModel.PriceCloseOrder));

                            TransactionModel.ClosePrice = OrderUpdate.UpdateData.AveragePrice;
                            TransactionModel.CloseTime = OrderUpdate.UpdateData.UpdateTime;
                            TransactionModel.Commission += OrderUpdate.UpdateData.Fee;
                            TransactionModel.Profit += OrderUpdate.UpdateData.RealizedProfit;
                            AddTransaction();

                            if (StrategyModel.IsLong)
                            {
                                StrategyModel.LongPlus += 1;
                            }
                            else if (StrategyModel.IsShort)
                            {
                                StrategyModel.ShortPlus += 1;
                            }
                            CloseOrder();
                        }
                        if (StrategyModel.IsLong && OrderUpdate.UpdateData.Side == OrderSide.Buy || StrategyModel.IsShort && OrderUpdate.UpdateData.Side == OrderSide.Sell)
                        {
                            StrategyModel.IsOpenOrder = true;
                            Quantity = OrderUpdate.UpdateData.Quantity;
                            Commission += OrderUpdate.UpdateData.Fee;
                            if (!StrategyModel.IsPartiallyFilled)
                            {
                                OpenOrder(OrderUpdate.UpdateData.OrderId, OrderUpdate.UpdateData.AveragePrice, OrderUpdate.UpdateData.Side, OrderUpdate.UpdateData.Quantity, OrderUpdate.UpdateData.UpdateTime, Commission);
                            }
                        }
                    }
                }
                // Write log
                string json = JsonConvert.SerializeObject(OrderUpdate.UpdateData);
                WriteLog(json);
            }
        }
        private async void PartiallyFilledOpenOrder(long orderId, decimal price, OrderSide side, DateTime time)
        {
            await Task.Run(async() => {
                await Task.Delay(1000);
                OpenOrder(orderId, price, side, Quantity, time, Commission);
                StrategyModel.IsPartiallyFilled = false;
            });
        }
        private void OpenOrder(long orderId, decimal price, OrderSide side, decimal quantity, DateTime time, decimal commission)
        {

            TransactionModel = new();
            TransactionModel.Id = orderId;
            if (side == OrderSide.Buy) TransactionModel.IsLong = true;
            TransactionModel.OpenPrice = price;
            TransactionModel.Quantity = quantity;
            TransactionModel.OpenTime = time;
            TransactionModel.Commission = commission;


            double priceDouble = Decimal.ToDouble(price);
            StrategyModel.PriceOpenOrder = priceDouble;
            decimal takeProfitPrice;
            OrderSide orderSide;
            if (side == OrderSide.Buy)
            {
                takeProfitPrice = RoundPriceDecimal(price + (price * Convert.ToDecimal(StrategyModel.TakeProfit) / 100));
                StrategyModel.StopLossOpenOrder = priceDouble - (priceDouble * StrategyModel.StopLoss / 100);
                orderSide = OrderSide.Sell;
                StrategyModel.HistoryModel.PointsIsOpenLong.Add((time.ToOADate(), priceDouble));
            }
            else
            {
                takeProfitPrice = RoundPriceDecimal(price - (price * Convert.ToDecimal(StrategyModel.TakeProfit) / 100));
                StrategyModel.StopLossOpenOrder = priceDouble + (priceDouble * StrategyModel.StopLoss / 100);
                orderSide = OrderSide.Buy;
                StrategyModel.HistoryModel.PointsIsOpenShort.Add((time.ToOADate(), priceDouble));
            }
            StrategyModel.TakeProfitOpenOrder = Decimal.ToDouble(takeProfitPrice);

            TakeProfitOrderId = OpenOrderTakeProfitAsync(orderSide, takeProfitPrice, quantity).Result;
        }
        private void CloseOrder()
        {
            StrategyModel.IsWaitRestartDalay = true;
            StrategyModel.IsOpenOrder = false;
            RestartDelay();
        }
        private async void RestartDelay()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(2000);
                ClearOrdersToSymbol();
                await Task.Delay(StrategyModel.RestartDalay);
                LimitOrderId = 0;
                StrategyModel.IsWaitRestartDalay = false;
            });
        }
        private async void CheckFollowPriceDelayAsync()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(StrategyModel.FollowPriceDelay);
                SetBufferAndDistance(SymbolModel.PriceDouble);
                SetBufferAndDistanceToChart();
                StrategyModel.IsFollowPriceDelay = false;
            });
        }

        private void WriteLog(string text)
        {
            try
            {
                if (!Directory.Exists(_pathLog)) Directory.CreateDirectory(_pathLog);
                File.AppendAllText($"{_pathLog}{StrategyModel.Name}_{StrategyModel.Distance}_{StrategyModel.Buffer}_{StrategyModel.TakeProfit}_{StrategyModel.StopLoss}_{StrategyModel.FollowPriceDelay}",$"{DateTime.Now} {text}\n");
            }
            catch { }
        }
    }
}
