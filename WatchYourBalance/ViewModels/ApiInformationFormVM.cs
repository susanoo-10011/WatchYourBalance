using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    public class ApiInformationFormVM
    {

        private string _GetApiKey;
        public string GetApiKey
        {
            get
            {
                if (ApiJson.ApiKeys() is null || ApiJson.ApiKeys().ApiKey == null) return "0";
                _GetApiKey = ApiJson.ApiKeys().ApiKey;
                return _GetApiKey;
            }
        }

        private string _GetApiSecret;
        public string GetApiSecret
        {
            get
            {
                if (ApiJson.ApiKeys() is null || ApiJson.ApiKeys().ApiSecret == null) return "0";
                _GetApiSecret = ApiJson.ApiKeys().ApiSecret;
                return _GetApiSecret;
            }
        }


    }
}
