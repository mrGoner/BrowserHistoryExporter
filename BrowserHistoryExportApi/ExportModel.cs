using System;
using System.Collections.Generic;

namespace BrowserHistoryExportApi
{
    [Serializable]
    public class HistoryCollection : List<History>, ICloneable
    {
        public object Clone()
        {
            var clonedObject = new HistoryCollection();

            foreach(var item in this)
            {
                clonedObject.Add(new History(item.Url, item.Title, item.Date));
            }

            return clonedObject;
        }
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