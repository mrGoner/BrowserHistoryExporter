using System;
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
    public class ConvertWizardViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        public List<string> SupportBrowsers { get; }
        public string SelectedBrowser { get; set; }
        public object DateFrom { get; set; }
        public object DateTill { get; set; }
        public bool IsDateSelected { get; set; }
        public delegate void SaveHistoryCollection(HistoryCollection _histories);
        public delegate void HistoryLoadHandler(HistoryCollection _collection);
        public event HistoryLoadHandler LoadHistoryEvent;
        public event SaveHistoryCollection SaveHistoryEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private BrowserExportApi m_exportApi;
        private string m_selectedPath;
        private RelayCommand m_convertCommand;
        private bool m_isConvert;
        private RelayCommand m_openFileCommand;
        private bool m_modelValid;

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

        public string SelectedPath
        {
            get => m_selectedPath;
            set
            {
                m_selectedPath = value;
                OnPropertyChanged(nameof(SelectedPath));
            }
        }

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

        public bool ModelValid
        {
            get => m_modelValid;
            set
            {
                if(m_modelValid != value)
                {
                    m_modelValid = value;
                    OnPropertyChanged(nameof(ModelValid));
                }
            }
        }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "SelectedPath":
                        if (!File.Exists(m_selectedPath))
                            error ="Choose a valid path!";
                        break;
                    case "SelectedBrowser":
                        if (SelectedBrowser == null)
                            error ="Browser not select!";
                        break;
                }

                ModelValid = !File.Exists(m_selectedPath) || SelectedBrowser == null;

                return error;
            }
        }

        private async void Convert()
        {
            IsConvert = true;

            var convertedHistory = await Task<HistoryCollection>.Factory.StartNew(() => 
            {
                try
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

                    return m_exportApi.Export(SelectedPath, SelectedBrowser, dateFrom, dateTill);
                }
                catch
                {
                    return null;
                }
            });

            IsConvert = false;

            if(convertedHistory == null)
            {
                MessageBox.Show("Error was occured!");
                return;
            }

            var saveResult = MessageBox.Show($"Successfully converted {convertedHistory.Count} histories. Save?", 
                "Operation completed", MessageBoxButton.YesNo);

            if (saveResult == MessageBoxResult.Yes)
                SaveHistoryEvent?.Invoke(convertedHistory);

            var openResult = MessageBox.Show("Open converted histories in the histrory view?", 
                "Question", MessageBoxButton.YesNo);

            if (openResult == MessageBoxResult.Yes)
                LoadHistoryEvent?.Invoke(convertedHistory);
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
