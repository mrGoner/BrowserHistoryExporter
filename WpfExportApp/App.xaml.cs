using System;
using System.Windows;

namespace WpfExportApp
{
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled error was occured: {((Exception)e.ExceptionObject).Message}", "Error!",
                     MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
