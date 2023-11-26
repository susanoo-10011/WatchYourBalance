using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;
using WatchYourBalance.Models.SaveSettings;
using WatchYourBalance.ViewModels.ApiVM;

namespace WatchYourBalance.ViewModels
{
    class GetApiVM : ObservableObject
    {
        public ICommand UpdateBalanceCommand { get; private set; }
        public GetApiVM() 
        {
            UpdateBalanceCommand = new RelayCommand(async _ => await UpdateBalanceAsync());

        }
        public string APISecret
        {
            get { return SaveReturnApi.ReturnApiSecret(); }
            
            set
            {
                SaveReturnApi.SaveApiSecret(value);
                OnPropertyChanged();
            }
        }

        public string APIKey
        {
            get { return SaveReturnApi.ReturnApiKey(); }
            set
            {
                SaveReturnApi.SaveApiKey(value);
                OnPropertyChanged();
            }
        }

        

        public async Task UpdateBalanceAsync()
        {
            ApiBinance aPIKeys = new ApiBinance();

            await aPIKeys.GetAccountData(SaveReturnApi.ReturnApiKey(), SaveReturnApi.ReturnApiSecret());
        }
    }
}
