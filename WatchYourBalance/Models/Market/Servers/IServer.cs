using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Models.Market.Servers
{
    //public interface IServer
    //{

    //}

    /// <summary>
    /// статус сервера
    /// </summary>
    public enum ServerConnectStatus
    {
        /// <summary>
        /// connected
        /// подключен
        /// </summary>
        Connect,

        /// <summary>
        /// disconnected
        /// отключен
        /// </summary>
        Disconnect,
    }
}
