using System.Collections.Generic;
using System.Linq;
using System;
using BrowserHistoryExportApi.Serializers.Interfaces;
using BrowserHistoryExportApi.Serializers;
using System.IO;

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
                new XmlModelSerializer(),
                new JsonModelSerializer()
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
            if (File.Exists(_pathToHistory))
            {
                var exporter = m_currentExporters.FirstOrDefault(_x => _x.BrowserName == _exporterName);

                if (exporter != null)
                {
                    var historyCollection = exporter.Export(_pathToHistory, _from, _until);

                    return historyCollection;
                }

                throw new InvalidOperationException($"Could not found exporter with name: {_exporterName}");
            }
            else
            {
                throw new FileNotFoundException($"File {_pathToHistory} not found!");
            }
        }

        public void SaveHistory(HistoryCollection _history, string _extention, string _pathToSave)
        {
            if (string.IsNullOrWhiteSpace(_extention))
                throw new ArgumentNullException(nameof(_extention));
            
            if (_history == null)
                throw new ArgumentNullException(nameof(_history));

            var serializer = m_currentSerializers.FirstOrDefault(_x => _x.Extention == _extention);

            if(serializer != null)
            {
                var data = serializer.Serialize(_history);
                File.WriteAllText(_pathToSave, data);
            }
            else
            {
                throw new InvalidOperationException($"Serializer for {_extention} not found!");   
            }
        }

        public string[] GetSupportBrowsers()
        {
            return m_currentExporters.Select(_x => _x.BrowserName).ToArray();
        }

        public string[] GetSupportExportExtentions()
        {
            return m_currentSerializers.Select(_x => _x.Extention).ToArray();
        }

        public HistoryCollection LoadHistory(string _pathToFile)
		{
			if (!File.Exists(_pathToFile))
				throw new FileNotFoundException($"File {_pathToFile} not found!");
			      
			string extention = Path.GetExtension(_pathToFile);

			var currentSerrializer = m_currentSerializers.FirstOrDefault(_x => _x.Extention == extention);

			if (currentSerrializer == null)
				throw new InvalidOperationException($"Serializer for extention {extention} not found!");

            var data = File.ReadAllText(_pathToFile);

			return currentSerrializer.Deserialize(data);
		}
    }
}