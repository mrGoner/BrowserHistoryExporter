using BrowserHistoryExportApi;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WpfExportApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private BrowserExportApi m_browserExportApi;
        private bool m_isLoading;
        private HistoryViewModel m_historyModel;
        private ICommand m_openCommand;
        private ICommand m_wizardCommand;
        private string m_loadFilter;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            m_browserExportApi = new BrowserExportApi();
            m_loadFilter = BuildSaveFilter();
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
                return m_openCommand ?? (m_openCommand = new RelayCommand(command => OpenFile()));
            }
        }

        public ICommand OpenWizardCommand
        {
            get
            {
                return m_wizardCommand ?? (m_wizardCommand = new RelayCommand(command => OpenWizard()));
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

        private void OpenWizard()
        {
            var wizardVm = new ConvertWizardViewModel(m_browserExportApi);
            var wizardWindow = new WizardWindow
            {
                DataContext = wizardVm
            };

            wizardWindow.ShowDialog();
        }

        private async void LoadModel(string _path)
        {
            IsLoading = true;

            await Task.Factory.StartNew(() =>
            {
                var historyCollection = m_browserExportApi.LoadHistory(_path);
                m_historyModel = new HistoryViewModel(historyCollection);
                m_historyModel.OnExportCommandClicked += M_historyModel_OnExportCommandClicked;

            }).ConfigureAwait(true);

            IsLoading = false;

            if (m_historyModel != null)
                OnPropertyChanged(nameof(HistoryModel));
        }

        private void M_historyModel_OnExportCommandClicked(IList<History> _histories)
        {
            if(_histories != null)
            {
                var saveFileDlg = new SaveFileDialog
                {
                    Filter = m_loadFilter,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                };

                var chooseResult = saveFileDlg.ShowDialog();

                if(chooseResult.HasValue && chooseResult.Value)
                {
                    var exportHistory = new HistoryCollection();
                    exportHistory.AddRange(_histories);
                    m_browserExportApi.SaveHistory(exportHistory,
                        Path.GetExtension(saveFileDlg.FileName), saveFileDlg.FileName);

                    MessageBox.Show("Успешно сохранено!");
                }
            }
        }

        private string BuildLoadFilter()
        {
            var extentions = m_browserExportApi.GetSupportExportExtentions();

            string filter = "Supported files | ";

            foreach(var extention in extentions)
            {
                filter += $"*{extention};";
            }

            return filter;
        }

        private string BuildSaveFilter()
        {
            var extentions = m_browserExportApi.GetSupportExportExtentions();

            string filter = string.Empty;

            foreach(var extention in extentions)
            {
                filter += $"({extention})|*{extention}|";
            }

            if (filter.EndsWith("|"))
               filter = filter.Remove(filter.LastIndexOf("|"), 1);

            return filter;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
