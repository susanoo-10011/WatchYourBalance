﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    class MainVM : ObservableObject
    {

        public RelayCommand GetApiViewCommand { get; set; }
        public RelayCommand StatisticsViewCommand { get; set; }
        public RelayCommand JournalViewCommand { get; set; }
        public RelayCommand TradesViewCommand { get; set; }
        public RelayCommand ConnectionListViewCommand { get; set; }

        public GetApiVM GetApiVM { get; set; }
        public StatisticsVM StatisticsVM { get; set; }
        public JournalVM JournalVM { get; set; }
        public TradesVM TradesVM { get; set; }
        public ConnectionListVM ConnectionListVM { get; set; }

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

        public ICommand CloseAppCommand {  get; set; }
        private void CloseApp()
        {
            Application.Current.Shutdown();
        }

        public MainVM()
        {

            GetApiVM = new GetApiVM();
            StatisticsVM = StatisticsVM.Instance;
            JournalVM = new JournalVM();
            TradesVM = new TradesVM();
            ConnectionListVM = ConnectionListVM.Instance;

            CurrentView = ConnectionListVM;

            ConnectionListViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConnectionListVM;
            });

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

            CloseAppCommand = new RelayCommand(o => CloseApp());
        }
    }
}
