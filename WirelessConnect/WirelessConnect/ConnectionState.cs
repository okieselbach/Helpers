using System;
using System.ComponentModel;

namespace WirelessConnect
{
    public class ConnectionState : INotifyPropertyChanged
    {
        private string wiredStateColor;
        private string wirelessStateColor;
        private string internetAccessStateColor;
        private string internetAccessStateButtonColor;
        public event EventHandler CheckForInternetReady;
        public event EventHandler InternetAccessable;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Wired
        {
            get { return wiredStateColor; }
            set
            {
                wiredStateColor = value;
                OnPropertyChanged(nameof(Wired));
                if (value.Equals("green", StringComparison.OrdinalIgnoreCase)) OnCheckForInternetReady();
            }
        }

        public string Wireless
        {
            get { return wirelessStateColor; }
            set
            {
                wirelessStateColor = value;
                OnPropertyChanged(nameof(Wireless));
                if (value.Equals("green", StringComparison.OrdinalIgnoreCase)) OnCheckForInternetReady();
            }
        }
        public string InternetAccess
        {
            get { return internetAccessStateColor; }
            set
            {
                internetAccessStateColor = value;
                InternetAccessButtonColor = value;
                OnPropertyChanged(nameof(InternetAccess));
                if (value.Equals("green", StringComparison.OrdinalIgnoreCase)) OnInternetAccess(new EventArgs());
            }
        }

        public string InternetAccessButtonColor
        {
            get { return internetAccessStateButtonColor; }
            set
            {
                if (value.Equals("green", StringComparison.OrdinalIgnoreCase))
                {
                    internetAccessStateButtonColor = value;
                }
                else
                {
                    internetAccessStateButtonColor = "#FFF0F0F0";
                }
                OnPropertyChanged(nameof(InternetAccessButtonColor));
            }
        }

        public ConnectionState()
        {
            Wired = "Red";
            Wireless = "Red";
            InternetAccess = "Red";
        }

        protected void OnCheckForInternetReady()
        {
            CheckForInternetReady?.Invoke(this, new EventArgs());
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnInternetAccess(EventArgs e)
        {
            InternetAccessable?.Invoke(this, e);
        }
    }
}
