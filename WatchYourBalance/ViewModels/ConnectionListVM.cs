using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            LoadFromJson();
            CreateСonnectionCommand = new RelayCommand(o => ShowWindow());
            UpdateCollectionEvent += SaveToJson;
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

        public ICommand CreateСonnectionCommand { get; set; }
        private void ShowWindow()
        {
            GetApiWindow getApiView = new GetApiWindow();
            getApiView.Show();
        }

        private ObservableCollection<ApiInformationFormVM> _ApiInformationFormVMList = new ObservableCollection<ApiInformationFormVM>();
        public ObservableCollection<ApiInformationFormVM> ApiInformationFormVMList
        {
            get { return _ApiInformationFormVMList; }
            set
            {
                _ApiInformationFormVMList = value;
                UpdateCollectionEvent?.Invoke();
                OnPropertyChanged();
            }
        }

        private event Action UpdateCollectionEvent;
        public void SaveToJson()
        {
            string folderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JsonFiles";

            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "CollectionApiInfoFormVM");

            var json = JsonConvert.SerializeObject(_ApiInformationFormVMList);

            File.WriteAllText(filePath, json);

        }

        private void LoadFromJson()
        {
            string filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JsonFiles" + "\\CollectionApiInfoFormVM";
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _ApiInformationFormVMList = JsonConvert.DeserializeObject<ObservableCollection<ApiInformationFormVM>>(json);
            }
        }
    }
}
