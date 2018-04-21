using System;

namespace BrowserHistoryExportApi
{
    public interface IExporter
    {
        string BrowserName { get; }
        ExportModel Export(string _pathToFile, DateTime _from, DateTime _until);
    }
}