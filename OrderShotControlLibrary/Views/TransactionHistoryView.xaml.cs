using Library.Models;
using System.Windows;

namespace OrderShotControlLibrary.Views
{
    public partial class TransactionHistoryView : Window
    {
        public StrategyModel StrategyModel { get; set; }
        public TransactionHistoryView(StrategyModel strategyModel)
        {
            StrategyModel = strategyModel;
            InitializeComponent();
            DataContext = this;
            Title = $"{StrategyModel.Name} | Distance: {StrategyModel.Distance} | Buffer: {StrategyModel.Buffer} | TakeProfit: {StrategyModel.TakeProfit} | StopLoss: {StrategyModel.StopLoss} | Delay: {StrategyModel.FollowPriceDelay}";
        }
    }
}
