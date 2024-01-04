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
                for (int i = 0; i < ConnectionListVM.Instance.ApiInformationFormVMList.Count; i++)
                {
                    if (ConnectionListVM.Instance.ApiInformationFormVMList[i].Name == Name)
                    {
                        throw new Exception("Такое имя уже существует!");
                    }
                }

                ApiJson.ApiSerializeJson(ApiKey, ApiSecret);

                BinanceServerFuturesRealization realization = BinanceServerFuturesRealization.Instance();
                realization.Connect();
                if (realization.ServerStatus == ServerConnectStatus.Connect)
                {
                    

                    ConnectionListVM.Instance.ApiInformationFormVMList.Add(new ApiInformationFormVM()
                    {
                        Name = Name,
                        GetApiKey = ApiKey,
                        GetApiSecret = ApiSecret,
                    });

                    ConnectionListVM.Instance.SaveToJson();

                    CloseGetApiWindow();
                }
                else
                {
                    throw new Exception("Ошибка подключения");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

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

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }


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
