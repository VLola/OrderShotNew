using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrderShotLibrary.Models
{
    public class StatisticsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is StatisticsModel))
            {
                return false;
            }
            return (this.Name == ((StatisticsModel)obj).Name)
                && (this.Distance == ((StatisticsModel)obj).Distance)
                && (this.Buffer == ((StatisticsModel)obj).Buffer)
                && (this.TakeProfit == ((StatisticsModel)obj).TakeProfit)
                && (this.StopLoss == ((StatisticsModel)obj).StopLoss)
                && (this.FollowPriceDelay == ((StatisticsModel)obj).FollowPriceDelay);
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
        private double _distance { get; set; }
        public double Distance
        {
            get { return _distance; }
            set
            {
                _distance = value;
                OnPropertyChanged("Distance");
            }
        }
        private double _buffer { get; set; }
        public double Buffer
        {
            get { return _buffer; }
            set
            {
                _buffer = value;
                OnPropertyChanged("Buffer");
            }
        }
        private double _takeProfit { get; set; }
        public double TakeProfit
        {
            get { return _takeProfit; }
            set
            {
                _takeProfit = value;
                OnPropertyChanged("TakeProfit");
            }
        }
        private double _stopLoss { get; set; }
        public double StopLoss
        {
            get { return _stopLoss; }
            set
            {
                _stopLoss = value;
                OnPropertyChanged("StopLoss");
            }
        }
        private int _followPriceDelay { get; set; }
        public int FollowPriceDelay
        {
            get { return _followPriceDelay; }
            set
            {
                _followPriceDelay = value;
                OnPropertyChanged("FollowPriceDelay");
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
                if (_shortMinus == 0) ShortWin = _shortPlus  * 100;
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
