using System;
using BrowserHistoryExportApi.Serializers.Interfaces;
using System.Xml.Serialization;
using System.IO;

namespace BrowserHistoryExportApi.Serializers
{
    public class XmlModelSerializer : IModelSerializer
    {
        private XmlSerializer m_xmlSerializer;

        public XmlModelSerializer()
        {
            m_xmlSerializer = new XmlSerializer(typeof(HistoryCollection));
        }

        public HistoryCollection Deserialize(string _data)
        {
            if (string.IsNullOrWhiteSpace(_data))
                throw new ArgumentException("Data can not be null or empty!");
            
            using (var reader = new StringReader(_data))
            {
                return (HistoryCollection)m_xmlSerializer.Deserialize(reader);
            }
        }

        public string Serialize(HistoryCollection _model)
        {
            if (_model == null)
                throw new ArgumentNullException(nameof(_model));
            
            using (var writer = new StringWriter())
            {
                m_xmlSerializer.Serialize(writer, _model);

                return writer.ToString();
            }
        }
    }
}