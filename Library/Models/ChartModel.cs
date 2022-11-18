using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class ChartModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public double[] ScatterUpperBufer { get; set; } = new double[2];
        public double[] ScatterLowerBufer { get; set; } = new double[2];
        public double[] ScatterUpperDistance { get; set; } = new double[2];
        public double[] ScatterLowerDistance { get; set; } = new double[2];
        public double[] ScatterTakeProfit { get; set; } = new double[2];
        public double[] ScatterStopLoss { get; set; } = new double[2];
        public double[] ScatterX { get; set; } = new double[2];
        public double[] ScatterXTpSl { get; set; } = new double[2];
        private bool _isRun { get; set; }
        public bool IsRun
        {
            get { return _isRun; }
            set
            {
                _isRun = value;
                OnPropertyChanged("IsRun");
            }
        }
        private bool _isAutoAxis { get; set; } = true;
        public bool IsAutoAxis
        {
            get { return _isAutoAxis; }
            set
            {
                _isAutoAxis = value;
                OnPropertyChanged("IsAutoAxis");
            }
        }
        private bool _isClose { get; set; }
        public bool IsClose
        {
            get { return _isClose; }
            set
            {
                _isClose = value;
                OnPropertyChanged("IsClose");
            }
        }
    }
}
