using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkExplorer.Data
{
    public class NetworkNode : INotifyPropertyChanged
    {
        private string ipAdress;
        public string IPAdress
        {
            get
            {
                return ipAdress;
            }
            set
            {
                if (value == ipAdress)
                    return;

                ipAdress = value;

                OnPropertyChanged("IPAdress");
            }
        }

        private string mac;

        public string? MAC 
        {
            get
            {
                return mac;
            }
            set
            {
                if (value == mac)
                    return;

                mac = value;

                OnPropertyChanged("MAC");
            }
        }

        private string name;

        public string? Name 
        {
            get
            {
                return name;
            }
            set
            {
                if (value == name)
                    return;

                name = value;

                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
