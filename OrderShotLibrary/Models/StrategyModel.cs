using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrderShotLibrary.Models
{
    public class StrategyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
        private int _longPlus { get; set; } = 0;
        public int LongPlus
        {
            get { return _longPlus; }
            set
            {
                _longPlus = value;
                OnPropertyChanged("LongPlus");
            }
        }
        private int _longMinus { get; set; } = 0;
        public int LongMinus
        {
            get { return _longMinus; }
            set
            {
                _longMinus = value;
                OnPropertyChanged("LongMinus");
            }
        }
        private int _shortPlus { get; set; } = 0;
        public int ShortPlus
        {
            get { return _shortPlus; }
            set
            {
                _shortPlus = value;
                OnPropertyChanged("ShortPlus");
            }
        }
        private int _shortMinus { get; set; } = 0;
        public int ShortMinus
        {
            get { return _shortMinus; }
            set
            {
                _shortMinus = value;
                OnPropertyChanged("ShortMinus");
            }
        }
        private int _longWin { get; set; } = 0;
        public int LongWin
        {
            get { return _longWin; }
            set
            {
                _longWin = value;
                OnPropertyChanged("LongWin");
            }
        }
        private int _shortWin { get; set; } = 0;
        public int ShortWin
        {
            get { return _shortWin; }
            set
            {
                _shortWin = value;
                OnPropertyChanged("ShortWin");
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
    }
}
