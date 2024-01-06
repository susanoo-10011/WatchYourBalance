using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Core;
using WatchYourBalance.Entity;
using WatchYourBalance.Models.Market.Servers.Binance.Futures;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.ViewModels
{
    public class StatisticsVM : ObservableObject
    {
        private static StatisticsVM instance;
        private static readonly object lockObject = new object();

        private StatisticsVM()
        {
            BinanceServerFuturesRealization binanceServer = BinanceServerFuturesRealization.Instance();
            binanceServer.AccountEvent += UpdateAccountInfo;
        }

        public static StatisticsVM Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new StatisticsVM();
                        }
                    }
                }
                return instance;
            }
        }

        private void UpdateAccountInfo(AccountResponseFutures account)
        {
            _GetCurrentOrder = account.positions.First().symbol;
            OnPropertyChanged();



            _GetTotalWalletBalance = account.totalWalletBalance;
            OnPropertyChanged(nameof(GetTotalWalletBalance));

            _GetCanDeposit = account.canDeposit;
            OnPropertyChanged(nameof(GetCanDeposit));


            _GetTotalMarginBalance = account.totalMarginBalance;
            OnPropertyChanged(nameof(GetTotalMarginBalance));
        }


        private string _GetCurrentOrder;
        public string GetCurrentOrder
        {
            get { return _GetCurrentOrder; }
            set
            {
                _GetCurrentOrder = value;
                OnPropertyChanged();
            }
        }
        private string _GetTotalWalletBalance;
        public string GetTotalWalletBalance
        {
            get
            { return _GetTotalWalletBalance; }
            set 
            {
                _GetTotalWalletBalance = value;
                OnPropertyChanged();
            }
        }

        private string _GetCanDeposit;
        public string GetCanDeposit
        {
            get
            { return _GetCanDeposit; }
            set
            {
                _GetCanDeposit = value;
                OnPropertyChanged();
            }
        }

        private string _GetTotalMarginBalance;
        public string GetTotalMarginBalance
        {
            get
            { return _GetTotalMarginBalance; }
            set
            {
                _GetTotalMarginBalance = value;
                OnPropertyChanged();
            }
        }
    }
}
