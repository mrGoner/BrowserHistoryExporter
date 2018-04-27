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
        public IList<History> SelectedHistories { get; set; }

        private HistoryCollection m_historyCollection;
        private RelayCommand m_openUrlCommand;
        private RelayCommand m_findHistoryCommand;
        private RelayCommand m_copyCommand;
        private RelayCommand m_searchCommand;
        private RelayCommand m_selectedChanged;
        private RelayCommand m_denySearch;
        private bool m_isSearched;

        public bool IsSearched
        {
            get
            {
                return m_isSearched;
            }
            set
            {
                if (m_isSearched != value)
                    m_isSearched = value;

                OnPropertyChanged(nameof(IsSearched));
            }
        }

        public HistoryViewModel(HistoryCollection _collection)
        {
            m_historyCollection = (HistoryCollection)_collection.Clone();
            CurrentHistoryCollection = _collection ?? throw new ArgumentNullException(nameof(_collection));
        }

        #region Commands

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

        public ICommand SelectedChanged
        {
            get
            {
                return m_selectedChanged ??
                    (m_selectedChanged = new RelayCommand(_items =>
                    {
                        var castedItems = ((IList<object>)_items).Cast<History>();
                        ViewSelectedItemChanged(castedItems.ToList());
                    }));
            }
        }

        public ICommand ClearSearchResult
        {
            get
            {
                return m_denySearch ?? (m_denySearch = new RelayCommand(_command => DenySearch()));
            }
        }

        #endregion

        private void OpenUrl()
        {
            if (SelectedHistories != null)
                Process.Start(SelectedHistories.First().Url);

            //Class not registered exception!
        }

        private void CopyToClipboard()
        {
            Clipboard.SetText(SelectedHistories.First().Url);
            //Many?
        }

        private void SearchItems(string _text)
        {
            var clonedHistory = (HistoryCollection)m_historyCollection.Clone();
            var result = clonedHistory.Where(
                _x => _x.Title.ToLower().Contains(_text.ToLower())).ToList();

            CurrentHistoryCollection.Clear();

            foreach(var item in result)
            {
                CurrentHistoryCollection.Add(item);
            }

            IsSearched = true;

            //async maybe
            //improve search like case insensetive and more
        }

        private void ViewSelectedItemChanged(List<History> _list)
        {
            SelectedHistories = _list;
        }

        private void DenySearch()
        {
            var clonedHistory = (HistoryCollection)m_historyCollection.Clone();

            CurrentHistoryCollection.Clear();

            foreach(var history in clonedHistory)
            {
                CurrentHistoryCollection.Add(history);
            }
            IsSearched = false;
            //async maybe?
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
