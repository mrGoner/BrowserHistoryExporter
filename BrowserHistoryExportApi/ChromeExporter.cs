using System;
using System.Data;

namespace BrowserHistoryExportApi
{
    public class ChromeExporter : IExporter
    {
        public string BrowserName => "Chrome";

        public ExportModel Export(string _pathToFile, DateTime _from, DateTime _until)
        {

            return null;
        }

    }
}