using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Models.SaveSettings
{
    public class AccountData
    {
        public static decimal Balance;
        public static void ReadingResponse(string content)
        {
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

            foreach (var balance in jsonResponse)
            {
                if (balance.asset == "USDT")
                {
                    decimal balanceDecimal = Convert.ToDecimal(balance.balance);
                    Balance = balanceDecimal;
                }
            }
        }
    }
}
