using System.Windows;
using Microsoft.Win32;
using WpfExportApp.ViewModels;

namespace WpfExportApp
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
