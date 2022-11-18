using Library.Command;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class TransactionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public long Id { get; set; }
        public bool IsLong { get; set; }
        public bool IsPositive { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal Quantity { get; set; }
        public decimal Commission { get; set; }

        private decimal _profit { get; set; }
        public decimal Profit
        {
            get { return _profit; }
            set
            {
                _profit = value;
                Total = value - Commission;
            }
        }
        private decimal _total { get; set; }
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                if (value >= 0m) IsPositive = true;
            }
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
        private RelayCommand? _viewCommand;
        public RelayCommand ViewCommand
        {
            get { return _viewCommand ?? (_viewCommand = new RelayCommand(obj => { IsView = true; })); }
        }
    }
}
