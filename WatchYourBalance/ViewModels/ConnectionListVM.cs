using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WatchYourBalance.Core;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    public class ConnectionListVM : ObservableObject
    {
        private static ConnectionListVM instance;
        private static readonly object lockObject = new object();

        private ConnectionListVM()
        {
            CreateСonnectionCommand = new RelayCommand(o => ShowWindow());

            GetApiVM.AddApiInfoFormView += AddItemsControl;
        }

        public static ConnectionListVM Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new ConnectionListVM();
                        }
                    }
                }
                return instance;
            }
        }

        private ObservableCollection<ApiInformationFormView> _ApiInformationFormViewList = new ObservableCollection<ApiInformationFormView>();
        public ObservableCollection<ApiInformationFormView> ApiInformationFormViewList
        {
            get { return _ApiInformationFormViewList; }
            set
            {
                _ApiInformationFormViewList = value;
                OnPropertyChanged();
            }
        }

        private void AddItemsControl()
        {
            ApiInformationFormViewList.Add(new ApiInformationFormView());
        }

        public ICommand CreateСonnectionCommand { get; set; }

        private void ShowWindow()
        {
            GetApiWindow getApiView = new GetApiWindow();
            getApiView.Show();
        }
    }
}
