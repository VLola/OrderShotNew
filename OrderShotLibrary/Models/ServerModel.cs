using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrderShotLibrary.Models
{
    public class ServerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public List<ClientData>? Symbols { get; set; }
        private bool _isLoad { get; set; }
        public bool IsLoad
        {
            get { return _isLoad; }
            set
            {
                _isLoad = value;
                OnPropertyChanged("IsLoad");
            }
        }
    }
}
