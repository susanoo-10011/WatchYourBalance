using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;
using WatchYourBalance.Models.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    class GetApiVM : ObservableObject
    {
        public GetApiVM()
        {
            SaveApiCommand = new RelayCommand(o => SaveApi());
        }

        public ICommand SaveApiCommand { get; set; }

        private void SaveApi()
        {
            ApiSerialize.ApiSerializeJson(_ApiKey, _ApiSecret);
        }

        private string _ApiKey;
        public string ApiKey
        {
            get { return _ApiKey; }
            set 
            { 
                _ApiKey = value;
                OnPropertyChanged();
            }
        }

        private string _ApiSecret;
        public string ApiSecret
        {
            get { return _ApiSecret; }
            set
            {
                _ApiSecret = value;
                OnPropertyChanged();
            }
        }
    }
}
