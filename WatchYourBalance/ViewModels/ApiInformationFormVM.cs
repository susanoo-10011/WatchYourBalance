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
            DeleteCommand = new RelayCommand(o => Delete());
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
            realization.Connect(GetApiKey, GetApiSecret);
        }

        public ICommand DeleteCommand { get; set; }
        private void Delete()
        {
            for(int i = 0; i < ConnectionListVM.Instance.ApiInformationFormVMList.Count; i++)
            {
                if (ConnectionListVM.Instance.ApiInformationFormVMList[i].Name == Name)
                {
                    ConnectionListVM.Instance.ApiInformationFormVMList.RemoveAt(i);
                    ConnectionListVM.Instance.SaveToJson();
                    break;
                }
            }
        }
    }
}
