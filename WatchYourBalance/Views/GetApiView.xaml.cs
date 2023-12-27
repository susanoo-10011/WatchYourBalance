using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WatchYourBalance.Views
{
    public partial class GetApiView : Window
    {
        public GetApiView()
        {
            InitializeComponent();
        }

        private void Toolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
