using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class ServerModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private List<ClientData>? _symbols { get; set; }
        public List<ClientData>? Symbols
        {
            get { return _symbols; }
            set
            {
                _symbols = value;
                OnPropertyChanged("Symbols");
            }
        }
    }
}
