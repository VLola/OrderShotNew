using Library.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class StrategyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public ObservableCollection<TransactionModel> TransactionHistory { get; set; } = new();
        public List<(double x, double y)> PointsIsBuyer { get; set; } = new();
        public List<(double x, double y)> PointsIsMaker { get; set; } = new();
        private int _restartDalay { get; set; } = 10000;
        public int RestartDalay
        {
            get { return _restartDalay; }
            set
            {
                _restartDalay = value;
                OnPropertyChanged("RestartDalay");
            }
        }
        private bool _isWaitRestartDalay { get; set; }
        public bool IsWaitRestartDalay
        {
            get { return _isWaitRestartDalay; }
            set
            {
                _isWaitRestartDalay = value;
                OnPropertyChanged("IsWaitRestartDalay");
            }
        }
        private bool _isStop { get; set; } = false;
        public bool IsStop
        {
            get { return _isStop; }
            set
            {
                _isStop = value;
                OnPropertyChanged("IsStop");
            }
        }

        private decimal _orderSize { get; set; } 
        public decimal OrderSize
        {
            get { return _orderSize; }
            set
            {
                _orderSize = value;
                OnPropertyChanged("OrderSize");
            }
        }
        private decimal _quantity { get; set; }
        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }
        private bool _isPartiallyFilled { get; set; } = false;
        public bool IsPartiallyFilled
        {
            get { return _isPartiallyFilled; }
            set
            {
                _isPartiallyFilled = value;
                OnPropertyChanged("IsPartiallyFilled");
            }
        }
        private bool _isLong { get; set; } = true;
        public bool IsLong
        {
            get { return _isLong; }
            set
            {
                _isLong = value;
                OnPropertyChanged("IsLong");
            }
        }
        private bool _isShort { get; set; } = true;
        public bool IsShort
        {
            get { return _isShort; }
            set
            {
                _isShort = value;
                OnPropertyChanged("IsShort");
            }
        }
        private double _upperBuffer { get; set; }
        public double UpperBuffer
        {
            get { return _upperBuffer; }
            set
            {
                _upperBuffer = value;
                OnPropertyChanged("UpperBuffer");
            }
        }
        private double _lowerBufer { get; set; }
        public double LowerBuffer
        {
            get { return _lowerBufer; }
            set
            {
                _lowerBufer = value;
                OnPropertyChanged("LowerBuffer");
            }
        }

        private double _upperDistance { get; set; }
        public double UpperDistance
        {
            get { return _upperDistance; }
            set
            {
                _upperDistance = value;
                OnPropertyChanged("UpperDistance");
            }
        }
        private double _lowerDistance { get; set; }
        public double LowerDistance
        {
            get { return _lowerDistance; }
            set
            {
                _lowerDistance = value;
                OnPropertyChanged("LowerDistance");
            }
        }
        private bool _isOpenOrder { get; set; }
        public bool IsOpenOrder
        {
            get { return _isOpenOrder; }
            set
            {
                _isOpenOrder = value;
                OnPropertyChanged("IsOpenOrder");
            }
        }
        private bool _isFollowPriceDelay { get; set; }
        public bool IsFollowPriceDelay
        {
            get { return _isFollowPriceDelay; }
            set
            {
                _isFollowPriceDelay = value;
                OnPropertyChanged("IsFollowPriceDelay");
            }
        }
        private double _priceOpenOrder { get; set; }
        public double PriceOpenOrder
        {
            get { return _priceOpenOrder; }
            set
            {
                _priceOpenOrder = value;
                OnPropertyChanged("PriceOpenOrder");
            }
        }
        private double _priceCloseOrder { get; set; }
        public double PriceCloseOrder
        {
            get { return _priceCloseOrder; }
            set
            {
                _priceCloseOrder = value;
                OnPropertyChanged("PriceCloseOrder");
            }
        }
        private bool _isLongOrder { get; set; }
        public bool IsLongOrder
        {
            get { return _isLongOrder; }
            set
            {
                _isLongOrder = value;
                OnPropertyChanged("IsLongOrder");
            }
        }
        private double _takeProfitOpenOrder { get; set; }
        public double TakeProfitOpenOrder
        {
            get { return _takeProfitOpenOrder; }
            set
            {
                _takeProfitOpenOrder = value;
                OnPropertyChanged("TakeProfitOpenOrder");
            }
        }
        private double _stopLossOpenOrder { get; set; }
        public double StopLossOpenOrder
        {
            get { return _stopLossOpenOrder; }
            set
            {
                _stopLossOpenOrder = value;
                OnPropertyChanged("StopLossOpenOrder");
            }
        }
        private bool _isPositiveBet { get; set; }
        public bool IsPositiveBet
        {
            get { return _isPositiveBet; }
            set
            {
                _isPositiveBet = value;
                OnPropertyChanged("IsPositiveBet");
            }
        }
        private bool _isMovingBufferAndDistance { get; set; }
        public bool IsMovingBufferAndDistance
        {
            get { return _isMovingBufferAndDistance; }
            set
            {
                _isMovingBufferAndDistance = value;
                OnPropertyChanged("IsMovingBufferAndDistance");
            }
        }
        private bool _isWait { get; set; }
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                _isWait = value;
                OnPropertyChanged("IsWait");
            }
        }

        // Statistics

        private RelayCommand? _visibleStrategyCommand;
        public RelayCommand VisibleStrategyCommand
        {
            get { return _visibleStrategyCommand ?? (_visibleStrategyCommand = new RelayCommand(obj => { IsView = true; })); }
        }
        private RelayCommand? _transactionHistoryCommand;
        public RelayCommand TransactionHistoryCommand
        {
            get { return _transactionHistoryCommand ?? (_transactionHistoryCommand = new RelayCommand(obj => { IsHistoryView = true; })); }
        }
        private string _name { get; set; }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private double _distance { get; set; } = 1;
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                OnPropertyChanged("Distance");
            }
        }
        private double _buffer { get; set; } = 0.5;
        public double Buffer
        {
            get { return _buffer; }
            set
            {
                _buffer = value;
                OnPropertyChanged("Buffer");
            }
        }
        private double _takeProfit { get; set; } = 0.15;
        public double TakeProfit
        {
            get { return _takeProfit; }
            set
            {
                _takeProfit = value;
                OnPropertyChanged("TakeProfit");
            }
        }
        private double _stopLoss { get; set; } = 0.45;
        public double StopLoss
        {
            get { return _stopLoss; }
            set
            {
                _stopLoss = value;
                OnPropertyChanged("StopLoss");
            }
        }
        private int _followPriceDelay { get; set; } = 1000;
        public int FollowPriceDelay
        {
            get { return _followPriceDelay; }
            set
            {
                _followPriceDelay = value;
                OnPropertyChanged("FollowPriceDelay");
            }
        }
        private decimal _minQuantity { get; set; }
        public decimal MinQuantity
        {
            get { return _minQuantity; }
            set
            {
                _minQuantity = value;
                OnPropertyChanged("MinQuantity");
            }
        }
        private decimal _stepSize { get; set; }
        public decimal StepSize
        {
            get { return _stepSize; }
            set
            {
                _stepSize = value;
                OnPropertyChanged("StepSize");
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
        private bool _isHistoryView { get; set; }
        public bool IsHistoryView
        {
            get { return _isHistoryView; }
            set
            {
                _isHistoryView = value;
                OnPropertyChanged("IsHistoryView");
            }
        }
        private double _win { get; set; } = 0;
        public double Win
        {
            get { return _win; }
            set
            {
                _win = value;
                OnPropertyChanged("Win");
            }
        }
        private double _longWin { get; set; } = 0;
        public double LongWin
        {
            get { return _longWin; }
            set
            {
                _longWin = value;
                OnPropertyChanged("LongWin");
            }
        }
        private double _shortWin { get; set; } = 0;
        public double ShortWin
        {
            get { return _shortWin; }
            set
            {
                _shortWin = value;
                OnPropertyChanged("ShortWin");
            }
        }
        private double _longPlus { get; set; } = 0;
        public double LongPlus
        {
            get { return _longPlus; }
            set
            {
                _longPlus = value;
                OnPropertyChanged("LongPlus");
                if (_longMinus == 0) LongWin = _longPlus * 100;
                else LongWin = _longPlus / _longMinus * 100;
                if ((_longMinus + _shortMinus) == 0) Win = (_longPlus + _shortPlus) / 1 * 100;
                else Win = (_longPlus + _shortPlus) / (_longMinus + _shortMinus) * 100;
            }
        }
        private double _longMinus { get; set; } = 0;
        public double LongMinus
        {
            get { return _longMinus; }
            set
            {
                _longMinus = value;
                OnPropertyChanged("LongMinus");
                LongWin = _longPlus / _longMinus * 100;
                if ((_longMinus + _shortMinus) == 0) Win = (_longPlus + _shortPlus) / 1 * 100;
                else Win = (_longPlus + _shortPlus) / (_longMinus + _shortMinus) * 100;
            }
        }
        private double _shortPlus { get; set; } = 0;
        public double ShortPlus
        {
            get { return _shortPlus; }
            set
            {
                _shortPlus = value;
                OnPropertyChanged("ShortPlus");
                if (_shortMinus == 0) ShortWin = _shortPlus * 100;
                else ShortWin = _shortPlus / _shortMinus * 100;
                if ((_longMinus + _shortMinus) == 0) Win = (_longPlus + _shortPlus) / 1 * 100;
                else Win = (_longPlus + _shortPlus) / (_longMinus + _shortMinus) * 100;
            }
        }
        private double _shortMinus { get; set; } = 0;
        public double ShortMinus
        {
            get { return _shortMinus; }
            set
            {
                _shortMinus = value;
                OnPropertyChanged("ShortMinus");
                ShortWin = _shortPlus / _shortMinus * 100;
                if ((_longMinus + _shortMinus) == 0) Win = (_longPlus + _shortPlus) / 1 * 100;
                else Win = (_longPlus + _shortPlus) / (_longMinus + _shortMinus) * 100;
            }
        }
    }
}
