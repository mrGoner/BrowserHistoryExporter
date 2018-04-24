using System;
using System.Collections.ObjectModel;

namespace BrowserHistoryExportApi
{
    [Serializable]
    public class HistoryCollection : ObservableCollection<History>
    {
        
    }

    [Serializable]
    public class History
    {
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public History(string _url, string _title, DateTime _date)
        {
            Date = _date;
            Url = _url;
            Title = _title;
        }

        public History()
        {
            
        }
    }
}