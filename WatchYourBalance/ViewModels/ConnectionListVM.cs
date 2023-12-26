using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using WatchYourBalance.Core;
using WatchYourBalance.Views;

namespace WatchYourBalance.ViewModels
{
    public class ConnectionListVM : ObservableObject
    {
        public ConnectionListVM()
        {
            CreateСonnectionCommand = new RelayCommand(o => ShowWindow());

            GetApiVM.AddApiInfoFormView += AddItemsControl;
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
            GetApiView getApiView = new GetApiView();
            getApiView.Show();
        }
    }
}
