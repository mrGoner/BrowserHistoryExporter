using System;
using System.Collections.Generic;

namespace BrowserHistoryExportApi
{
    [Serializable]
    public class HistoryCollection : List<History>
    {
        
    }

    [Serializable]
    public class History
    {
        public DateTime Date { get; }
        public Uri Url { get; }
        public string Title { get; }

        public History(Uri _url, string _title, DateTime _date)
        {
            Date = _date;
            Url = _url;
            Title = _title;
        }
    }
}