using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models.Market.Servers.Binance.Futures;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    public class ApiInformationFormVM : ObservableObject
    {
        public ApiInformationFormVM() 
        {
            СonnectionCommand = new RelayCommand(o => Сonnection());
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        private string _GetApiKey;
        public string GetApiKey
        {
            get
            {
                return _GetApiKey;
            }
            set 
            { 
                _GetApiKey = value; 
                OnPropertyChanged();
           }
        }

        private string _GetApiSecret;
        public string GetApiSecret
        {
            get
            {
                return _GetApiSecret;
            }
            set
            {
                _GetApiSecret = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public ICommand СonnectionCommand {  get; set; }

        private void Сonnection()
        {
            BinanceServerFuturesRealization realization = BinanceServerFuturesRealization.Instance();
            realization.Connect();
        }

        
    }
}
