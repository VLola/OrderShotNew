using Library.Models;
using System.Collections.ObjectModel;

namespace OrderShotServer.ViewModels
{
    public class StatisticsViewModel
    {
        public ObservableCollection<StrategyModel> StatisticsModels { get; set; } = new();

    }
}
