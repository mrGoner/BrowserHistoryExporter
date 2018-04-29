using BrowserHistoryExportApi;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WpfExportApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BrowserExportApi m_browserExportApi;
        private bool m_isLoading;
        private HistoryViewModel m_historyModel;
        private ICommand m_openCommand;
        private string m_loadFilter;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            m_browserExportApi = new BrowserExportApi();
            m_loadFilter = BuildFilter();
        }

        public bool IsLoading
        {
            get => m_isLoading;
            set
            {
                if (m_isLoading != value)
                    m_isLoading = value;

                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public HistoryViewModel HistoryModel { get => m_historyModel; }
        
        public ICommand OpenCommand
        {
            get
            {
                return m_openCommand ?? (m_openCommand = new CommandHandler(() => OpenFile(), true));
            }
        }

        private void OpenFile()
        {
            var openFileDlg = new OpenFileDialog
            {
                Filter = m_loadFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckPathExists = true
            };

            var chooseResult = openFileDlg.ShowDialog();

            if (chooseResult.HasValue && chooseResult.Value)
                LoadModel(openFileDlg.FileName);
        }

        private async void LoadModel(string _path)
        {
            IsLoading = true;

            await Task.Factory.StartNew(() =>
            {
                var historyCollection = m_browserExportApi.LoadHistory(_path);
                m_historyModel = new HistoryViewModel(historyCollection);

            }).ConfigureAwait(true);

            IsLoading = false;

            if (m_historyModel != null)
                OnPropertyChanged(nameof(HistoryModel));
        }

        private string BuildFilter()
        {
            var extentions = m_browserExportApi.GetSupportExportExtentions();

            string filter = "Supported files | ";

            foreach(var extention in extentions)
            {
                filter += $"*{extention};";
            }

            return filter;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
