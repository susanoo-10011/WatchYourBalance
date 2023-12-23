using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    class GetApiVM : ObservableObject
    {
        public GetApiVM()
        {
            UpdateData = new RelayCommand(o => SaveApiKey());
        }

        public ICommand UpdateData { get; set; }
        public void SaveApiKey()
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

        public string GetApi
        {
            get
            {
                if (ApiSerialize.ApiKeys() is null || ApiSerialize.ApiKeys().ApiKey == null) return "0";
                return ApiSerialize.ApiKeys().ApiKey;
            }
        }
    }
}
