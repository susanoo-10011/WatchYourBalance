using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Core;
using WatchYourBalance.Models.SaveSettings;

namespace WatchYourBalance.ViewModels
{
    internal class StatisticsVM :ObservableObject
    {
        public decimal Balance
        {
            get { return AccountData.Balance; }
            set
            {
                AccountData.Balance = value;
                OnPropertyChanged();
            }
        }
    }
}
