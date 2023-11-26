using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;
using WatchYourBalance.ViewModels.ApiVM;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    class MainVM : ObservableObject
    {

        public RelayCommand GetApiViewCommand { get; set; }
        public RelayCommand StatisticsViewCommand { get; set; }
        public RelayCommand JournalViewCommand { get; set; }
        public RelayCommand TradesViewCommand { get; set; }
        public RelayCommand EnterApiCommand { get; set; }


        public GetApiVM GetApiVM { get; set; }
        public StatisticsVM StatisticsVM { get; set; }
        public JournalVM JournalVM { get; set; }
        public TradesVM TradesVM { get; set; }
        public EnterApiVM EnterApiVM{ get; set; }

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainVM()
        {

            GetApiVM = new GetApiVM();
            StatisticsVM = new StatisticsVM();
            JournalVM = new JournalVM();
            TradesVM = new TradesVM();
            EnterApiVM = new EnterApiVM();

            CurrentView = GetApiVM;

            GetApiViewCommand = new RelayCommand(o =>
            {
                CurrentView = GetApiVM;
            });

            StatisticsViewCommand = new RelayCommand(o =>
            {
                CurrentView = StatisticsVM;
            });

            JournalViewCommand = new RelayCommand(o =>
            {
                CurrentView = JournalVM;
            });

            TradesViewCommand = new RelayCommand(o =>
            {
                CurrentView = TradesVM;
            });

            EnterApiCommand = new RelayCommand(o =>
            {
                CurrentView = EnterApiVM;
            });

        }

    }
}
