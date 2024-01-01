using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Entity
{
    public class Position
    {
        /// <summary>
        /// Transaction direction
        /// </summary>
        public enum Side
        {
            /// <summary>
            /// None
            /// </summary>
            None,

            /// <summary>
            /// Buy
            /// </summary>
            Buy,

            /// <summary>
            /// Sell
            /// </summary>
            Sell
        }
    }
}
