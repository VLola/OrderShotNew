using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.Models
{
    public class LoginModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public ObservableCollection<string> Clients { get; set; } = new();
        public string SelectedClient { get; set; }
        private bool _isLogin { get; set; } = false;
        public bool IsLogin
        {
            get { return _isLogin; }
            set
            {
                _isLogin = value;
                OnPropertyChanged("IsLogin");
            }
        }
        private bool _isTestnet { get; set; } = false;
        public bool IsTestnet
        {
            get { return _isTestnet; }
            set
            {
                _isTestnet = value;
                OnPropertyChanged("IsTestnet");
            }
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
        private string _apiKey { get; set; }
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value;
                OnPropertyChanged("ApiKey");
            }
        }
        private string _secretKey { get; set; }
        public string SecretKey
        {
            get { return _secretKey; }
            set
            {
                _secretKey = value;
                OnPropertyChanged("SecretKey");
            }
        }
    }
}
