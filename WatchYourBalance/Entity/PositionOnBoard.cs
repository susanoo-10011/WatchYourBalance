using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Entity
{
    /// <summary>
    /// Common position on the instrument on the exchange
    /// </summary>
    public class PositionOnBoard
    {
        /// <summary>
        /// Position at the beginning of the session
        /// </summary>
        public decimal ValueBegin;

        /// <summary>
        /// Current volume
        /// </summary>
        public decimal ValueCurrent;

        /// <summary>
        /// Blocked volume
        /// </summary>
        public decimal ValueBlocked;

        /// <summary>
        /// Tool for which the position is open
        /// </summary>
        public string SecurityNameCode;

        /// <summary>
        /// Portfolio on which the position is open
        /// </summary>
        public string PortfolioName;
    }
}
