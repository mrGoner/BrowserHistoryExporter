using System.Collections.Generic;
using System.Linq;
using System;
using BrowserHistoryExportApi.Serializers.Interfaces;
using BrowserHistoryExportApi.Serializers;

namespace BrowserHistoryExportApi
{
    public class BrowserExportApi
    {
        List<IExporter> m_currentExporters;
        List<IModelSerializer> m_currentSerializers;

        public BrowserExportApi()
        {
            m_currentExporters = new List<IExporter>
            {
                new ChromeExporter()
            };

            m_currentSerializers = new List<IModelSerializer>
            {
                new XmlModelSerializer()
            };
        }

        public void RegisterExporter(IExporter _exporter)
        {
            if (_exporter == null)
                throw new ArgumentNullException(nameof(_exporter));

            if (m_currentExporters.FirstOrDefault(_x => _x.BrowserName == _exporter.BrowserName) != null)
            {
                throw new InvalidOperationException(
                    $"Exporter with name {_exporter.BrowserName} already registered!");
            }
            else
            {
                m_currentExporters.Add(_exporter);
            }
        }

        public void RegisterSerializer(IModelSerializer _serializer)
        {
            if (_serializer == null)
                throw new ArgumentNullException(nameof(_serializer));
            
            if (m_currentSerializers.FirstOrDefault(_x=> _x.Extention == _serializer.Extention) != null)
            {
                throw new InvalidOperationException(
                    $"Serializer with same extention ({_serializer.Extention}) already registered!");
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

        public string[] GetSupportExportExtentions()
        {
            return m_currentSerializers.Select(_x => _x.Extention).ToArray();
        }
    }
}