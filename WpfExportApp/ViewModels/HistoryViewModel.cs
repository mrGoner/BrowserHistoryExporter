using System;
using BrowserHistoryExportApi;

namespace WpfExportApp.ViewModels
{
    public class HistoryViewModel
    {
        public HistoryCollection CurrentHistory { get; }

        public HistoryViewModel(HistoryCollection _collection)
        {
            CurrentHistory = _collection ?? throw new ArgumentNullException(nameof(_collection));
        }
    }
}
