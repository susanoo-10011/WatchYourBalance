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
            SaveApiCommand = new RelayCommand(o => SaveApi());
            CloseGatApiWindowCommand = new RelayCommand(o => CloseGatApiWindow());
        }

        #region Команды
        public ICommand SaveApiCommand { get; }

        private void SaveApi()
        {
            ApiSerialize.ApiSerializeJson(_ApiKey, _ApiSecret);

            //if(статус сервера == false) вернуть всплывающее окно с ошибкой
            bool check = true;
            if (check == true)
            {
                AddApiInfoFormView?.Invoke();
                CloseGatApiWindow();
            }
        }

        public static event Action AddApiInfoFormView;

        public ICommand CloseGatApiWindowCommand { get; }
        private void CloseGatApiWindow()
        {
            var window = Application.Current.Windows
           .OfType<Window>()
           .FirstOrDefault(x => x.DataContext == this);

            if (window != null)
            {
                window.Close();
            }
        }


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

        private string _GetApiKey;
        public string GetApiKey
        {
            get
            {
                if (ApiSerialize.ApiKeys() is null || ApiSerialize.ApiKeys().ApiKey == null) return "0";
                _GetApiKey = ApiSerialize.ApiKeys().ApiKey;
                return _GetApiKey;
            }
        }
        #endregion

    }
}
