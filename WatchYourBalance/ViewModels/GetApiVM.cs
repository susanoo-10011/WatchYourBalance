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
using WatchYourBalance.Models.Market.Servers;
using WatchYourBalance.Models.Market.Servers.Binance.Futures;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    public class GetApiVM : ObservableObject
    {
        public GetApiVM()
        {
            СonnectionСommand = new RelayCommand(o => Сonnection());
            CloseGetApiWindowCommand = new RelayCommand(o => CloseGetApiWindow());
        }

        #region Команды
        public ICommand СonnectionСommand { get; }

        private void Сonnection()
        {
            try
            {
                ApiJson.ApiSerializeJson(_ApiKey, _ApiSecret);

                BinanceServerFuturesRealization realization = BinanceServerFuturesRealization.Instance();
                realization.Connect();
                if (realization.ServerStatus == ServerConnectStatus.Connect)
                {
                    AddApiInfoFormView?.Invoke();
                    CloseGetApiWindow();
                }
                else
                {
                    MessageBox.Show("Ошибка подключения", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
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
