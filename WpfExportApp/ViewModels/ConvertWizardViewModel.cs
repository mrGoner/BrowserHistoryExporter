using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BrowserHistoryExportApi;
using Microsoft.Win32;

namespace WpfExportApp.ViewModels
{
    public class ConvertWizardViewModel : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        public List<string> SupportBrowsers { get; }
        public string SelectedBrowser { get; set; }

        public string SelectedPath
        {
            get => m_selectedPath;
            set
            {
                m_selectedPath = value;
                OnPropertyChanged(nameof(SelectedPath));
            }
        }

        public object DateFrom { get; set; }
        public object DateTill { get; set; }
        public bool IsDateSelected { get; set; }

        public bool IsConvert
        {
            get => m_isConvert;
            set
            {
                if (m_isConvert != value)
                {
                    m_isConvert = value;
                    OnPropertyChanged(nameof(IsConvert));
                }
            }
        }

        public delegate void ExportHandler();
        public event ExportHandler ExportEvent;
        public delegate void HistoryLoadHandler(ExportEventArgs _args);
        public event HistoryLoadHandler LoadHistoryEvent;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private BrowserExportApi m_exportApi;
        private string m_selectedPath;
        private RelayCommand m_convertCommand;
        private bool m_isConvert;
        private RelayCommand m_openFileCommand;

        public ConvertWizardViewModel(BrowserExportApi _exportApi)
        {

            m_exportApi = _exportApi ?? throw new ArgumentNullException(nameof(_exportApi));
            SupportBrowsers = _exportApi.GetSupportBrowsers().ToList();
        }

        public ICommand ConvertCommand
        {
            get
            {
                return m_convertCommand ?? (m_convertCommand = new RelayCommand(_command => Convert()));
            }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                return m_openFileCommand ?? (m_openFileCommand = new RelayCommand(_command => OpenFileDlg()));
            }
        }

        private void OpenFileDlg()
        {
            var fileDlg = new OpenFileDialog
            {
                Filter = "(All Files) | *.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                CheckFileExists = true,
                Multiselect = false
            };

            var result = fileDlg.ShowDialog();

            if (result.HasValue && result.Value)
                SelectedPath = fileDlg.FileName;
        }

        public bool HasErrors => !File.Exists(m_selectedPath) || SelectedBrowser == null;

        private async void Convert()
        {
            IsConvert = true;

            var convertedHistory = await Task<HistoryCollection>.Factory.StartNew(() => 
            {
                var dateFrom = DateTime.MinValue;
                var dateTill = DateTime.MaxValue;

                if (IsDateSelected)
                {
                    if (DateTime.TryParse(DateFrom?.ToString(), out var from))
                        dateFrom = from;
                    if (DateTime.TryParse(DateTill?.ToString(), out var till))
                        dateTill = till;
                }

                return  m_exportApi.Export(SelectedPath, SelectedBrowser, dateFrom, dateTill);
            });

            MessageBox.Show($"Successfully converted {convertedHistory.Count} histories");
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var errors = new List<string>();

            switch (propertyName)
            {
                case "SelectedPath":
                    if (!File.Exists(m_selectedPath))
                        errors.Add("Choose a valid path!");
                    break;
                case "SelectedBrowser":
                    if (SelectedBrowser == null)
                        errors.Add("Browser not select!");
                    break;
            }

            return errors;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class ExportEventArgs : EventArgs
    {
        public string BrowserName { get; }
        public string PathToFile { get; }

        public ExportEventArgs(string _pathToFile, string _browserName)
        {
            BrowserName = _browserName;
            PathToFile = _pathToFile;
        }
    }
}
