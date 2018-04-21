using System;

namespace BrowserHistoryExportApi
{
    public interface IExporter
    {
        string BrowserName { get; }
        HistoryCollection Export(string _pathToFile, DateTime _from, DateTime _until);
    }
}