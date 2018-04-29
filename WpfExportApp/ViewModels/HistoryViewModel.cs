using System;
using BrowserHistoryExportApi;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;

namespace WpfExportApp.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<History> CurrentHistoryCollection { get; private set; }
        public IList<History> SelectedHistories { get; set; }
        public object DateFrom { get; set; } = DateTime.Now;
        public object DateTill { get; set; } = DateTime.Now;
        public bool IsDateSearch { get; set; }

        private HistoryCollection m_historyCollection;
        private RelayCommand m_openUrlCommand;
        private RelayCommand m_copyCommand;
        private RelayCommand m_searchCommand;
        private RelayCommand m_selectedChanged;
        private RelayCommand m_denySearch;
        private bool m_isSearched;
        private bool m_isLoading;
        
        public bool IsSearched
        {
            get
            {
                return m_isSearched;
            }
            set
            {
                if (m_isSearched != value)
                {
                    m_isSearched = value;
                    OnPropertyChanged(nameof(IsSearched));
                }
            }
        }

        public bool IsLoading
        {
            get
            {
                return m_isLoading;
            }
            set
            {
                if (m_isLoading != value)
                {
                    m_isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public HistoryViewModel(HistoryCollection _collection)
        {
            if (_collection == null)
                throw new ArgumentNullException(nameof(_collection));

            m_historyCollection = (HistoryCollection)_collection.Clone();
            CurrentHistoryCollection = new ObservableCollection<History>(_collection);
        }


        #region Commands

        public ICommand OpenUrlCommand
        {
            get
            {
                return m_openUrlCommand ?? (m_openUrlCommand = new RelayCommand(_command => OpenUrl()));
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

        private async void SearchItems(string _text)
        {
            if (string.IsNullOrWhiteSpace(_text) && !IsDateSearch)
                return;

            IsLoading = true;

            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                var clonedHistory = (List<History>)m_historyCollection.Clone();

                if (IsDateSearch)
                {
                    DateTime from;
                    DateTime till;

                    if (DateTime.TryParse(DateFrom?.ToString(), out var dateFrom))
                        from = dateFrom;
                    else
                        from = DateTime.MinValue;

                    if (DateTime.TryParse(DateTill?.ToString(), out var dateTill))
                        till = dateTill;
                    else
                        till = DateTime.MaxValue;

                    clonedHistory = clonedHistory.Where(_x => _x.Date >= from && _x.Date <= till).ToList();
                }
                var result = clonedHistory.Where(
                         _x => _x.Title.ToLower().Contains(_text.ToLower())).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentHistoryCollection.Clear();

                    foreach (var item in result)
                    {
                        CurrentHistoryCollection.Add(item);
                    }
                });

            }).ConfigureAwait(true);

            IsSearched = true;
            IsLoading = false;
            
            //improve search like case insensetive and more
        }

        private void ViewSelectedItemChanged(List<History> _list)
        {
            SelectedHistories = _list;
        }

        private async void DenySearch()
        {
            IsLoading = true;

            await Task.Factory.StartNew(() =>
            {
                var clonedHistory = (HistoryCollection)m_historyCollection.Clone();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentHistoryCollection.Clear();

                    foreach (var history in clonedHistory)
                    {
                        CurrentHistoryCollection.Add(history);
                    }
                });
            }).ConfigureAwait(true);

            IsSearched = false;
            IsLoading = false;
        }

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
