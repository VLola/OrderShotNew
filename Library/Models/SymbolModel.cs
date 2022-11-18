using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class SymbolModel : INotifyPropertyChanged
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
        private decimal _price { get; set; }
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }
        private DateTime _time { get; set; }
        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }
        private decimal _usdt { get; set; } = 11m;
        public decimal Usdt
        {
            get { return _usdt; }
            set
            {
                _usdt = value;
                OnPropertyChanged("Usdt");
            }
        }
        private double _timeDouble { get; set; }
        public double TimeDouble
        {
            get { return _timeDouble; }
            set
            {
                _timeDouble = value;
                OnPropertyChanged("TimeDouble");
            }
        }
        private double _priceDouble { get; set; }
        public double PriceDouble
        {
            get { return _priceDouble; }
            set
            {
                _priceDouble = value;
                OnPropertyChanged("PriceDouble");
            }
        }
        private bool _buyerIsMaker { get; set; }
        public bool BuyerIsMaker
        {
            get { return _buyerIsMaker; }
            set
            {
                _buyerIsMaker = value;
                OnPropertyChanged("Price");
            }
        }
        private decimal _volume { get; set; }
        public decimal Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged("Volume");
            }
        }
        private decimal _minVolume { get; set; } = 50000000m;
        public decimal MinVolume
        {
            get { return _minVolume; }
            set
            {
                _minVolume = value;
                OnPropertyChanged("MinVolume");
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
        private decimal _tickSize { get; set; }
        public decimal TickSize
        {
            get { return _tickSize; }
            set
            {
                _tickSize = value;
                OnPropertyChanged("TickSize");
                int index = value.ToString().IndexOf("1");
                if(index == 0) RoundPrice = index;
                else RoundPrice = index - 1;
            }
        }
        private int _roundPrice { get; set; }
        public int RoundPrice
        {
            get { return _roundPrice; }
            set
            {
                _roundPrice = value;
                OnPropertyChanged("RoundPrice");
            }
        }

        private double _distanceSetting { get; set; } = 0.4;
        public double DistanceSetting
        {
            get { return _distanceSetting; }
            set
            {
                _distanceSetting = value;
                OnPropertyChanged("DistanceSetting");
            }
        }
        private double _bufferSetting { get; set; } = 0.2;
        public double BufferSetting
        {
            get { return _bufferSetting; }
            set
            {
                _bufferSetting = value;
                OnPropertyChanged("BufferSetting");
            }
        }
        private double _takeProfitSetting { get; set; } = 0.1;
        public double TakeProfitSetting
        {
            get { return _takeProfitSetting; }
            set
            {
                _takeProfitSetting = value;
                OnPropertyChanged("TakeProfitSetting");
            }
        }
        private double _stopLossSetting { get; set; } = 0.3;
        public double StopLossSetting
        {
            get { return _stopLossSetting; }
            set
            {
                _stopLossSetting = value;
                OnPropertyChanged("StopLossSetting");
            }
        }
        private int _followPriceDelaySetting { get; set; } = 1000;
        public int FollowPriceDelaySetting
        {
            get { return _followPriceDelaySetting; }
            set
            {
                _followPriceDelaySetting = value;
                OnPropertyChanged("FollowPriceDelaySetting");
            }
        }
        private bool _listenKeyExpired { get; set; }
        public bool ListenKeyExpired
        {
            get { return _listenKeyExpired; }
            set
            {
                _listenKeyExpired = value;
                OnPropertyChanged("ListenKeyExpired");
            }
        }
    }
}
