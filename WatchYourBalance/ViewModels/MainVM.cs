using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;

namespace WatchYourBalance.ViewModels
{
    class MainVM : ObservableObject
    {
        public ICommand UpdateBalanceCommand { get; private set; }

        public MainVM()
        {
            UpdateBalanceCommand = new RelayCommand(async _ => await UpdateBalanceAsync());
        }

        private string _APISecret;
        public string APISecret
        {
            get { return _APISecret; }
            set
            {
                _APISecret = value;
                OnPropertyChanged();
            }
        }

        private string _APIKey;
        public string APIKey
        {
            get { return _APIKey; }
            set
            {
                _APIKey = value;
                OnPropertyChanged();
            }
        }

        private decimal _balance;
        public decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                OnPropertyChanged(nameof(Balance));
            }
        }

        public async Task UpdateBalanceAsync()
        {
            ApiBinance aPIKeys = new ApiBinance();
            decimal newBalance = await aPIKeys.GetBalance(_APIKey, _APISecret);
            Balance = newBalance;
        }
    }
}
