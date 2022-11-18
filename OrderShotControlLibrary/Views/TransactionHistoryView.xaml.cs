using Library.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace OrderShotControlLibrary.Views
{
    public partial class TransactionHistoryView : Window
    {
        public ObservableCollection<TransactionModel> TransactionModels { get; set; }
        public TransactionHistoryView(ObservableCollection<TransactionModel> transactionModels, string name)
        {
            TransactionModels = transactionModels;
            InitializeComponent();
            DataContext = this;
            Title = name;
        }
    }
}
