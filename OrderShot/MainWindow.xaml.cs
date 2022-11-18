using OrderShot.ViewModels;
using ScottPlot;
using System.Windows;

namespace OrderShot
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
