using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Models.SaveSettings
{
    public class SaveReturnApi
    {
        private static string _ApiKey;
        private static string _ApiSecret;

        public static string ReturnApiKey()
        {
            if (_ApiKey == null) return null;

            return _ApiKey;
        }

        public static string ReturnApiSecret()
        {
            if (_ApiSecret == null) return null;

            return _ApiSecret;
        }

        public static void SaveApiSecret(string ApiSecret)
        {
            _ApiSecret = ApiSecret;
        }

        public static void SaveApiKey(string ApiKey)
        {
            _ApiKey = ApiKey;
        }
    }
}
