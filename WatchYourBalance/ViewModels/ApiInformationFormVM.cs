using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Models.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    public class ApiInformationFormVM
    {

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

        private string _GetApiSecret;
        public string GetApiSecret
        {
            get
            {
                if (ApiSerialize.ApiKeys() is null || ApiSerialize.ApiKeys().ApiSecret == null) return "0";
                _GetApiSecret = ApiSerialize.ApiKeys().ApiSecret;
                return _GetApiSecret;
            }
        }


    }
}
