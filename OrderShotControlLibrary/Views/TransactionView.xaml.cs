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
        public TransactionView(List<(double x, double y)> maker, List<(double x, double y)> buyer)
        {
            InitializeComponent();
            DataContext = this;
            MyPlot = new();
            MyPlot.Dispatcher.Invoke(new Action(() =>
            {
                MyPlot.Plot.RenderLock();

                foreach (var item in maker)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.Red, size: 4);
                }
                foreach (var item in buyer)
                {
                    MyPlot.Plot.AddPoint(x: item.x, y: item.y, color: Color.Green, size: 4);
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
