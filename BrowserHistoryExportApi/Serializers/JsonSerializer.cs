using BrowserHistoryExportApi.Serializers.Interfaces;

namespace BrowserHistoryExportApi.Serializers
{
    class JsonSerializer : IModelSerializer
    {
        public string Extention => ".json";

        public HistoryCollection Deserialize(string _data)
        {
            return null;
        }

        public string Serialize(HistoryCollection _model)
        {
            return null;
        }
    }
}
