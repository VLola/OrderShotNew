using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using Library.Models;
using Library.Command;

namespace OrderShot.ViewModels
{
    public class StrategyViewModel : INotifyPropertyChanged
    {
        public void Dispose()
        {
            Unsubscribe();
            ChartViewModel.Unsubscribe();
            ChartViewModel.ChartModel.IsClose = true;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private bool _isView { get; set; }
        public bool IsView
        {
            get { return _isView; }
            set
            {
                _isView = value;
                OnPropertyChanged("IsView");
            }
        }
        private bool _isClose { get; set; }
        public bool IsClose
        {
            get { return _isClose; }
            set
            {
                _isClose = value;
                OnPropertyChanged("IsClose");
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
        public StrategyModel StrategyModel { get; set; }
        public ChartViewModel ChartViewModel { get; set; }
        public SymbolModel SymbolModel { get; set; }
        public StrategyViewModel(SymbolModel symbolModel, ClientData clientData)
        {
            StrategyModel = new();
            SymbolModel = symbolModel;
            StarStrategy(clientData);
        }
        private async void StarStrategy(ClientData clientData)
        {
            await Task.Run(async() =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if(SymbolModel.PriceDouble > 0)
                    {
                        StrategyModel.Name = SymbolModel.Name;
                        StrategyModel.Distance = clientData.Distance;
                        StrategyModel.Buffer = clientData.Buffer;
                        StrategyModel.TakeProfit = clientData.TakeProfit;
                        StrategyModel.StopLoss = clientData.StopLoss;
                        StrategyModel.FollowPriceDelay = clientData.FollowPriceDelay;
                        Subscription();
                        SetBufferAndDistance(SymbolModel.PriceDouble);
                        ChartViewModel = new(StrategyModel, SymbolModel);
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
            StrategyModel.PropertyChanged -= StrategyModel_PropertyChanged;
        }
        private void CloseStrategy()
        {
            IsClose = true;
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
            if (e.PropertyName == "Distance" || e.PropertyName == "Buffer")
            {
                SetBufferAndDistance(SymbolModel.PriceDouble);
                SetBufferAndDistanceToChart();
            }
            else if (e.PropertyName == "IsView")
            {
                VisibleStrategy();
            }
        }

        private void Symbol_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Price" && SymbolModel.Volume > SymbolModel.MinVolume)
            {
                if (StrategyModel.IsOpenOrder)
                {
                    if (StrategyModel.IsLongOrder)
                    {
                        if (SymbolModel.PriceDouble > StrategyModel.TakeProfitOpenOrder)
                        {
                            StrategyModel.IsPositiveBet = true;
                            StrategyModel.LongPlus += 1;
                            CloseOrder();
                        }
                        else if (SymbolModel.PriceDouble < StrategyModel.StopLossOpenOrder)
                        {
                            StrategyModel.IsPositiveBet = false;
                            StrategyModel.LongMinus += 1;
                            CloseOrder();
                        }
                    }
                    else
                    {
                        if (SymbolModel.PriceDouble < StrategyModel.TakeProfitOpenOrder)
                        {
                            StrategyModel.IsPositiveBet = true;
                            StrategyModel.ShortPlus += 1;
                            CloseOrder();
                        }
                        else if (SymbolModel.PriceDouble > StrategyModel.StopLossOpenOrder)
                        {
                            StrategyModel.IsPositiveBet = false;
                            StrategyModel.ShortMinus += 1;
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
                            StrategyModel.IsLongOrder = false;
                            CheckFollowPriceDelayAsync();
                        }
                    }
                    else if (SymbolModel.PriceDouble < StrategyModel.LowerBuffer)
                    {
                        if (!StrategyModel.IsFollowPriceDelay)
                        {
                            StrategyModel.IsFollowPriceDelay = true;
                            StrategyModel.IsLongOrder = true;
                            CheckFollowPriceDelayAsync();
                        }
                    }
                    if (StrategyModel.IsFollowPriceDelay)
                    {
                        if (SymbolModel.PriceDouble > StrategyModel.UpperDistance || SymbolModel.PriceDouble < StrategyModel.LowerDistance)
                        {
                            if (StrategyModel.IsLongOrder)
                            {
                                if (StrategyModel.IsLong && !StrategyModel.IsWaitRestartDalay)
                                {
                                    StrategyModel.PriceOpenOrder = StrategyModel.LowerDistance;
                                    StrategyModel.TakeProfitOpenOrder = StrategyModel.PriceOpenOrder + (StrategyModel.PriceOpenOrder * StrategyModel.TakeProfit / 100);
                                    StrategyModel.StopLossOpenOrder = StrategyModel.PriceOpenOrder - (StrategyModel.PriceOpenOrder * StrategyModel.StopLoss / 100);
                                    StrategyModel.IsOpenOrder = true;
                                }
                                else
                                {
                                    SetBufferAndDistance(SymbolModel.PriceDouble);
                                    SetBufferAndDistanceToChart();
                                }
                            }
                            else
                            {
                                if (StrategyModel.IsShort && !StrategyModel.IsWaitRestartDalay)
                                {
                                    StrategyModel.PriceOpenOrder = StrategyModel.UpperDistance;
                                    StrategyModel.TakeProfitOpenOrder = StrategyModel.PriceOpenOrder - (StrategyModel.PriceOpenOrder * StrategyModel.TakeProfit / 100);
                                    StrategyModel.StopLossOpenOrder = StrategyModel.PriceOpenOrder + (StrategyModel.PriceOpenOrder * StrategyModel.StopLoss / 100);
                                    StrategyModel.IsOpenOrder = true;
                                }
                                else
                                {
                                    SetBufferAndDistance(SymbolModel.PriceDouble);
                                    SetBufferAndDistanceToChart();
                                }
                            }
                        }
                    }
                }
            }
            
        }
        private async void CheckFollowPriceDelayAsync()
        {
            await Task.Run(async () => {
                await Task.Delay(StrategyModel.FollowPriceDelay);
                if(!StrategyModel.IsOpenOrder)
                {
                    SetBufferAndDistance(SymbolModel.PriceDouble); 
                    SetBufferAndDistanceToChart();
                }
                StrategyModel.IsFollowPriceDelay = false;
            });
        }
        private void SetBufferAndDistance(double price)
        {
            StrategyModel.UpperBuffer = price + (price * StrategyModel.Buffer / 200);
            StrategyModel.LowerBuffer = price - (price * StrategyModel.Buffer / 200);
            StrategyModel.UpperDistance = price + (price * StrategyModel.Distance / 100);
            StrategyModel.LowerDistance = price - (price * StrategyModel.Distance / 100);
        }
        private void SetBufferAndDistanceToChart()
        {
            StrategyModel.IsMovingBufferAndDistance = true;
        }
        private void CloseOrder()
        {
            StrategyModel.IsWaitRestartDalay = true;
            StrategyModel.IsOpenOrder = false;
            SetBufferAndDistance(SymbolModel.PriceDouble);
            SetBufferAndDistanceToChart();
            RestartDelay();
        }
        private async void RestartDelay()
        {
            await Task.Run(async() =>
            {
                await Task.Delay(StrategyModel.RestartDalay);
                StrategyModel.IsWaitRestartDalay = false;
            });
        }
    }
}
