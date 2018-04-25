using System;
using BrowserHistoryExportApi;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Linq;

namespace WpfExportApp.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public HistoryCollection CurrentHistoryCollection { get; private set; }
        public History SelectedHistory { get; set; }
        public IEnumerable<History> SelectedHistories { get; set; }

        private HistoryCollection m_historyCollection;
        private RelayCommand m_openUrlCommand;
        private RelayCommand m_findHistoryCommand;
        private RelayCommand m_copyCommand;
        private RelayCommand m_searchCommand;

        public HistoryViewModel(HistoryCollection _collection)
        {
            m_historyCollection = _collection ?? throw new ArgumentNullException(nameof(_collection));
            CurrentHistoryCollection = m_historyCollection;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ICommand OpenUrlCommand
        {
            get
            {
                return m_openUrlCommand ?? (m_openUrlCommand = new RelayCommand(_command => OpenUrl()));
            }
        }

        public ICommand FindHistoryCommand
        {
            get
            {
                return m_findHistoryCommand ?? (m_findHistoryCommand = new RelayCommand(_command => { }));
            }
        }

        public ICommand CopyCommand
        {
            get
            {
                return m_copyCommand ?? (m_copyCommand = new RelayCommand(_command => CopyToClipboard()));
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return m_searchCommand ?? 
                    (m_searchCommand = new RelayCommand(_stringArgs => SearchItems(_stringArgs.ToString())));
            }
        }

        private void OpenUrl()
        {
            if (SelectedHistory != null)
                Process.Start(SelectedHistory.Url);

            //Class not registered exception!
        }

        private void CopyToClipboard()
        {
            Clipboard.SetText(SelectedHistory.Url);
            //Many?
        }

        private void SearchItems(string _text)
        {
            var result = m_historyCollection.Where(_x => _x.Title.Contains(_text)).ToList();

            CurrentHistoryCollection.Clear();

            foreach(var item in result)
            {
                CurrentHistoryCollection.Add(item);
            }

            //improve search like case insensetive and more
        }
    }
}
