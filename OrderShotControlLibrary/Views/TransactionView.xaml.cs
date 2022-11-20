using Library.Models;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrderShotControlLibrary.Views
{
    /// <summary>
    /// Логика взаимодействия для TransactionView.xaml
    /// </summary>
    public partial class TransactionView : Window
    {
        public WpfPlot MyPlot { get; set; }
        public TransactionView(HistoryModel HistoryModel)
        {
            InitializeComponent();
            DataContext = this;
            MyPlot = new();
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();

                foreach (var item in HistoryModel.PointsIsMaker)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.Red, size: 4);
                }
                foreach (var item in HistoryModel.PointsIsBuyer)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.Green, size: 4);
                }
                foreach (var item in HistoryModel.PointsIsOpenLong)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.LimeGreen, size: 12, shape: MarkerShape.filledDiamond);
                }
                foreach (var item in HistoryModel.PointsIsOpenShort)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.OrangeRed, size: 12, shape: MarkerShape.filledDiamond);
                }
                foreach (var item in HistoryModel.PointsIsClosePositive)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.LightBlue, size: 12, shape: MarkerShape.filledDiamond);
                }
                foreach (var item in HistoryModel.PointsIsCloseNegative)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.Orange, size: 12, shape: MarkerShape.eks);
                }

                MyPlot.Plot.Style(ScottPlot.Style.Gray2);
                MyPlot.Plot.XAxis.TickLabelFormat("HH:mm:ss", dateTimeFormat: true);
                MyPlot.Plot.RenderUnlock();
            }));
        }
        private void Loading()
        {

        }
    }
}
