using System.Collections.Generic;
using System.Linq;
using System;

namespace BrowserHistoryExportApi
{
    public class BrowserExportApi
    {
        List<IExporter> m_currentExporters;

        public BrowserExportApi()
        {
            m_currentExporters = new List<IExporter>();

            m_currentExporters.Add(new ChromeExporter());
        }

        public void RegisterExporter(IExporter _exporter)
        {
            if (_exporter == null)
                throw new ArgumentNullException(nameof(_exporter));

            if (m_currentExporters.FirstOrDefault(_x => _x.BrowserName == _exporter.BrowserName) != null)
            {
                throw new InvalidOperationException($"Exporter with name {_exporter.BrowserName} already registered!");
            }
            else
            {
                m_currentExporters.Add(_exporter);
            }
        }

        public HistoryCollection Export(string _pathToHistory, 
                                        string _exporterName, DateTime _from, DateTime _until)
        {
            var exporter = m_currentExporters.FirstOrDefault(_x => _x.BrowserName == _exporterName);

            if(exporter != null)
            {
                var historyCollection = exporter.Export(_pathToHistory, _from, _until);

                return historyCollection;
            }

            throw new InvalidOperationException($"Could not found exporter with name: {_exporterName}");
        }

        public string[] GetSupportBrowsers()
        {
            return m_currentExporters.Select(_x => _x.BrowserName).ToArray();
        }
    }
}