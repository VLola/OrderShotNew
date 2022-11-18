using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class MainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private string _nameSetting { get; set; }
        public string NameSetting
        {
            get { return _nameSetting; }
            set
            {
                _nameSetting = value;
                OnPropertyChanged("NameSetting");
            }
        }
        private bool _isLong { get; set; } = false;
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
        private bool _isRunChart { get; set; }
        public bool IsRunChart
        {
            get { return _isRunChart; }
            set
            {
                _isRunChart = value;
                OnPropertyChanged("IsRunChart");
            }
        }
    }
}
