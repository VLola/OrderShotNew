using Library.Models;
using ScottPlot;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace OrderShot.ViewModels
{
    public class ChartViewModel
    {
        public WpfPlot MyPlot { get; set; }
        private const int _delay = 100;
        private const double _second10 = 0.00011574074596865103;
        private const double _second60 = 0.0006944444394321181;
        public ChartModel ChartModel { get; set; }
        public SymbolModel SymbolModel { get; set; }
        public StrategyModel StrategyModel { get; set; }
        public ChartViewModel(StrategyModel strategyModel, SymbolModel symbolModel)
        {
            SymbolModel = symbolModel;
            ChartModel = new();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                MyPlot = new();
            });
            StrategyModel = strategyModel; 
            Subscription();
            Loading();
            Run();
        }
        public void Subscription()
        {
            StrategyModel.PropertyChanged += StrategyModel_PropertyChanged;
            SymbolModel.PropertyChanged += SymbolModel_PropertyChanged;
            ChartModel.PropertyChanged += ChartModel_PropertyChanged;
        }
        public void Unsubscribe()
        {
            SymbolModel.PropertyChanged -= SymbolModel_PropertyChanged;
            StrategyModel.PropertyChanged -= StrategyModel_PropertyChanged;
            ChartModel.PropertyChanged -= ChartModel_PropertyChanged;
        }
        private void ChartModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsRun")
            {
                if (ChartModel.IsRun)
                {
                    SetY(StrategyModel.LowerDistance, StrategyModel.UpperDistance);
                }
            }
        }

        private void SymbolModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ChartModel.IsRun)
            {
                if (e.PropertyName == "Price")
                {
                    if (SymbolModel.BuyerIsMaker)
                    {
                        AddPointRed(SymbolModel.TimeDouble, SymbolModel.PriceDouble);
                    }
                    else
                    {
                        AddPointGreen(SymbolModel.TimeDouble, SymbolModel.PriceDouble);
                    }
                }
            }
        }

        private void StrategyModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ChartModel.IsRun)
            {
                if (e.PropertyName == "IsOpenOrder")
                {
                    if (StrategyModel.IsOpenOrder)
                    {
                        if (StrategyModel.IsLongOrder)
                        {
                            AddOrder(SymbolModel.TimeDouble, StrategyModel.PriceOpenOrder, Color.LimeGreen, MarkerShape.filledDiamond);
                        }
                        else
                        {
                            AddOrder(SymbolModel.TimeDouble, StrategyModel.PriceOpenOrder, Color.OrangeRed, MarkerShape.filledDiamond);
                        }
                        ChartModel.ScatterTakeProfit[0] = ChartModel.ScatterTakeProfit[1] = StrategyModel.TakeProfitOpenOrder;
                        ChartModel.ScatterStopLoss[0] = ChartModel.ScatterStopLoss[1] = StrategyModel.StopLossOpenOrder;
                    }
                    else
                    {
                        if (StrategyModel.IsLongOrder)
                        {
                            if (StrategyModel.IsPositiveBet)
                            {
                                AddOrder(SymbolModel.TimeDouble, StrategyModel.TakeProfitOpenOrder, Color.LightBlue, MarkerShape.filledDiamond);
                            }
                            else
                            {
                                AddOrder(SymbolModel.TimeDouble, StrategyModel.StopLossOpenOrder, Color.Orange, MarkerShape.eks);
                            }
                        }
                        else
                        {
                            if (StrategyModel.IsPositiveBet)
                            {
                                AddOrder(SymbolModel.TimeDouble, StrategyModel.TakeProfitOpenOrder, Color.LightBlue, MarkerShape.filledDiamond);
                            }
                            else
                            {
                                AddOrder(SymbolModel.TimeDouble, StrategyModel.StopLossOpenOrder, Color.Orange, MarkerShape.eks);
                            }
                        }
                        ChartModel.ScatterTakeProfit[0] = ChartModel.ScatterTakeProfit[1] = 0;
                        ChartModel.ScatterStopLoss[0] = ChartModel.ScatterStopLoss[1] = 0;
                    }
                }
                else if(e.PropertyName == "IsMovingBufferAndDistance")
                {
                    if (StrategyModel.IsMovingBufferAndDistance)
                    {
                        SetBufferAndDistanceToChart(StrategyModel.UpperBuffer, StrategyModel.LowerBuffer, StrategyModel.UpperDistance, StrategyModel.LowerDistance);
                    }
                }
                else if (e.PropertyName == "TakeProfit")
                {
                    if (StrategyModel.IsLongOrder)
                    {
                        StrategyModel.TakeProfitOpenOrder = StrategyModel.PriceOpenOrder + (StrategyModel.PriceOpenOrder * StrategyModel.TakeProfit / 100);
                        ChartModel.ScatterTakeProfit[0] = ChartModel.ScatterTakeProfit[1] = StrategyModel.TakeProfitOpenOrder;
                    }
                    else
                    {
                        StrategyModel.TakeProfitOpenOrder = StrategyModel.PriceOpenOrder - (StrategyModel.PriceOpenOrder * StrategyModel.TakeProfit / 100);
                        ChartModel.ScatterTakeProfit[0] = ChartModel.ScatterTakeProfit[1] = StrategyModel.TakeProfitOpenOrder;
                    }
                }
                else if (e.PropertyName == "StopLoss")
                {
                    if (StrategyModel.IsLongOrder)
                    {
                        StrategyModel.StopLossOpenOrder = StrategyModel.PriceOpenOrder - (StrategyModel.PriceOpenOrder * StrategyModel.StopLoss / 100);
                        ChartModel.ScatterStopLoss[0] = ChartModel.ScatterStopLoss[1] = StrategyModel.StopLossOpenOrder;
                    }
                    else
                    {
                        StrategyModel.StopLossOpenOrder = StrategyModel.PriceOpenOrder + (StrategyModel.PriceOpenOrder * StrategyModel.StopLoss / 100);
                        ChartModel.ScatterStopLoss[0] = ChartModel.ScatterStopLoss[1] = StrategyModel.StopLossOpenOrder;
                    }
                }
            }
        }

        private async void Run()
        {
            await Task.Run(async () =>
            {
                while (ChartModel != null && !ChartModel.IsClose)
                {
                    await Task.Delay(_delay);
                    if (ChartModel != null && ChartModel.IsRun)
                    {
                        try
                        {
                            SetX();
                            MyPlot.Dispatcher.Invoke(new Action(() =>
                            {
                                MyPlot.Plot.RenderLock();
                                MyPlot.Render();
                                MyPlot.Plot.RenderUnlock();
                            }));
                        }
                        catch { }
                    }
                }
            });
        }
        private void Loading()
        {
            ChartModel.ScatterX[0] = ChartModel.ScatterX[1] = DateTime.UtcNow.ToOADate();
            ChartModel.ScatterXTpSl[0] = ChartModel.ScatterXTpSl[1] = DateTime.UtcNow.ToOADate();
            ChartModel.ScatterUpperBufer[0] = ChartModel.ScatterUpperBufer[1] = StrategyModel.UpperBuffer;
            ChartModel.ScatterLowerBufer[0] = ChartModel.ScatterLowerBufer[1] = StrategyModel.LowerBuffer;
            ChartModel.ScatterUpperDistance[0] = ChartModel.ScatterUpperDistance[1] = StrategyModel.UpperDistance;
            ChartModel.ScatterLowerDistance[0] = ChartModel.ScatterLowerDistance[1] = StrategyModel.LowerDistance;
            ChartModel.ScatterTakeProfit[0] = ChartModel.ScatterTakeProfit[1] = StrategyModel.TakeProfitOpenOrder;
            ChartModel.ScatterStopLoss[0] = ChartModel.ScatterStopLoss[1] = StrategyModel.StopLossOpenOrder;

            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.AddScatter(ChartModel.ScatterX, ChartModel.ScatterUpperDistance, color: Color.Red, lineWidth: 1, markerSize: 0);
                MyPlot.Plot.AddScatter(ChartModel.ScatterX, ChartModel.ScatterLowerDistance, color: Color.Green, lineWidth: 1, markerSize: 0);
                MyPlot.Plot.AddScatter(ChartModel.ScatterX, ChartModel.ScatterUpperBufer, color: Color.CadetBlue, lineWidth: 1, markerSize: 0, lineStyle: LineStyle.Dash);
                MyPlot.Plot.AddScatter(ChartModel.ScatterX, ChartModel.ScatterLowerBufer, color: Color.CadetBlue, lineWidth: 1, markerSize: 0, lineStyle: LineStyle.Dash);
                MyPlot.Plot.AddScatter(ChartModel.ScatterXTpSl, ChartModel.ScatterTakeProfit, color: Color.SkyBlue, lineWidth: 1, markerSize: 0);
                MyPlot.Plot.AddScatter(ChartModel.ScatterXTpSl, ChartModel.ScatterStopLoss, color: Color.Yellow, lineWidth: 1, markerSize: 0);
                MyPlot.Plot.Style(ScottPlot.Style.Gray2);
                MyPlot.Plot.XAxis.TickLabelFormat("HH:mm:ss", dateTimeFormat: true);
                MyPlot.Plot.RenderUnlock();
            }));
        }
        public void AddPointRed(double time, double price)
        {
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.AddPoint(x: time, y: price, color: Color.Red, size: 4);
                MyPlot.Plot.RenderUnlock();
            }));
        }
        public void AddPointGreen(double time, double price)
        {
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.AddPoint(x: time, y: price, color: Color.Green, size: 4);
                MyPlot.Plot.RenderUnlock();
            }));
        }
        public void AddOrder(double time, double price, Color color, MarkerShape shape)
        {
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.AddPoint(x: time, y: price, color: color, size: 12, shape: shape);
                MyPlot.Plot.RenderUnlock();
            }));
        }
        public void SetBufferAndDistanceToChart(double upperBuffer, double lowerBuffer, double upperDistance, double lowerDistance)
        {
            double[] x = new double[2];
            x[0] = ChartModel.ScatterX[0];
            x[1] = ChartModel.ScatterX[1];
            double[] upperDistances = new double[2];
            upperDistances[0] = ChartModel.ScatterUpperDistance[0];
            upperDistances[1] = ChartModel.ScatterUpperDistance[1];
            double[] lowerDistances = new double[2];
            lowerDistances[0] = ChartModel.ScatterLowerDistance[0];
            lowerDistances[1] = ChartModel.ScatterLowerDistance[1];
            double[] upperBuffers = new double[2];
            upperBuffers[0] = ChartModel.ScatterUpperBufer[0];
            upperBuffers[1] = ChartModel.ScatterUpperBufer[1];
            double[] lowerBuffers = new double[2];
            lowerBuffers[0] = ChartModel.ScatterLowerBufer[0];
            lowerBuffers[1] = ChartModel.ScatterLowerBufer[1];


            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.AddScatter(x, upperDistances, color: System.Drawing.Color.Gray, lineWidth: 1, markerSize: 0);
                MyPlot.Plot.AddScatter(x, lowerDistances, color: System.Drawing.Color.Gray, lineWidth: 1, markerSize: 0);

                MyPlot.Plot.AddScatter(x, upperBuffers, color: System.Drawing.Color.Gray, lineWidth: 1, markerSize: 0, lineStyle: LineStyle.Dash);
                MyPlot.Plot.AddScatter(x, lowerBuffers, color: System.Drawing.Color.Gray, lineWidth: 1, markerSize: 0, lineStyle: LineStyle.Dash);
                MyPlot.Plot.RenderUnlock();
            }));

            ChartModel.ScatterX[0] = DateTime.UtcNow.ToOADate();
            ChartModel.ScatterUpperBufer[0] = ChartModel.ScatterUpperBufer[1] = upperBuffer;
            ChartModel.ScatterLowerBufer[0] = ChartModel.ScatterLowerBufer[1] = lowerBuffer;
            ChartModel.ScatterUpperDistance[0] = ChartModel.ScatterUpperDistance[1] = upperDistance;
            ChartModel.ScatterLowerDistance[0] = ChartModel.ScatterLowerDistance[1] = lowerDistance;

            SetY(lowerDistance, upperDistance);
        }
        private void SetY(double lowerDistance, double upperDistance)
        {
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.SetAxisLimitsY(yMin: lowerDistance - (lowerDistance / 300), yMax: upperDistance + (upperDistance / 300));
                MyPlot.Plot.RenderUnlock();
            }));
        }
        private void SetX()
        {
            double dateTime = DateTime.UtcNow.ToOADate();
            ChartModel.ScatterX[1] = dateTime;
            ChartModel.ScatterXTpSl[0] = dateTime;
            ChartModel.ScatterXTpSl[1] = dateTime + _second10;
            if(ChartModel.IsAutoAxis)AutoAxisX(dateTime);
        }
        private void AutoAxisX(double dateTime)
        {
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();
                MyPlot.Plot.SetAxisLimitsX(xMin: dateTime - _second60, xMax: dateTime + _second10);
                MyPlot.Plot.RenderUnlock();
            }));
        }
    }
}
