using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Models.Market
{
    public class ServerMaster
    {

    }

    public enum ServerType
    {
        /// <summary>
        /// cryptocurrency exchange Binance
        /// биржа криптовалют Binance
        /// </summary>
        Binance,

        /// <summary>
        /// cryptocurrency exchange Binance Futures
        /// биржа криптовалют Binance, секция фьючеры
        /// </summary>
        BinanceFutures
    }
}