using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Core;
using WatchYourBalance.Models.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    public class StatisticsVM :ObservableObject
    {
        public StatisticsVM() 
        {

        }

        private string _GetApiKey;
        public string GetApiKey
        {
            get
            {
                if (ApiSerialize.ApiKeys() is null || ApiSerialize.ApiKeys().ApiKey == null) return "0";
                _GetApiKey = ApiSerialize.ApiKeys().ApiKey;
                return _GetApiKey;
            }
        }
    }
}
