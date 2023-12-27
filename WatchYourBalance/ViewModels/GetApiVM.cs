using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Models;
using WatchYourBalance.Models.Servers.Binance.Futures.Entity;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    public class GetApiVM : ObservableObject
    {
        public GetApiVM()
        {
            СonnectionСommand = new RelayCommand(o => Сonnection());
            CloseGetApiWindowCommand = new RelayCommand(o => CloseGetApiWindow());
            //GetApiMinimizedCommand = new RelayCommand(o => GetApiWindowMinimized());
        }

        #region Команды
        public ICommand СonnectionСommand { get; }

        private void Сonnection()
        {
            ApiSerialize.ApiSerializeJson(_ApiKey, _ApiSecret);

            //if(статус сервера == false) вернуть всплывающее окно с ошибкой
            bool check = true;
            if (check == true)
            {
                AddApiInfoFormView?.Invoke();
                CloseGetApiWindow();
            }
        }

        public static event Action AddApiInfoFormView;

        public ICommand CloseGetApiWindowCommand { get; }
        private void CloseGetApiWindow()
        {
            var window = Application.Current.Windows
           .OfType<Window>()
           .FirstOrDefault(x => x.DataContext == this);

            if (window != null)
            {
                window.Close();
            }
        }

        //public ICommand GetApiMinimizedCommand { get; }
        //private void GetApiWindowMinimized()
        //{
        //    Application.Current.Windows[1].WindowState = WindowState.Minimized;
        //}

        #endregion

        #region Свойства
        private string _ApiKey;
        public string ApiKey
        {
            get { return _ApiKey; }
            set
            {
                _ApiKey = value;
                OnPropertyChanged();
            }
        }

        private string _ApiSecret;
        public string ApiSecret
        {
            get { return _ApiSecret; }
            set
            {
                _ApiSecret = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}
