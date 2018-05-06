using BrowserHistoryExportApi.Serializers.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BrowserHistoryExportApi.Serializers
{
    internal class JsonModelSerializer : IModelSerializer
    {
        //Save date in universal date?

        public string Extention => ".json";

        public HistoryCollection Deserialize(string _data)
        {
            if (string.IsNullOrWhiteSpace(_data))
                throw new ArgumentException("Data can not be null or empty!");

            var historyArray = JArray.Parse(_data);
            var historyCollection = new HistoryCollection();

            foreach(var history in historyArray)
            {
                historyCollection.Add(DeserializeHistory((JObject)history));
            }

            return historyCollection;
        }

        public string Serialize(HistoryCollection _model)
        {
            var jArray = new JArray();

            foreach(var history in _model)
            {
                jArray.Add(SerializeHistory(history));
            }

            return jArray.ToString(Formatting.Indented);
        }

        private JObject SerializeHistory(History _history)
        {
            var jHistory = new JObject
            {
                new JProperty("date", _history.Date),
                new JProperty("title", _history.Title),
                new JProperty("url", _history.Url)
            };

            return jHistory;
        }

        private History DeserializeHistory(JObject _jHistory)
        {
            if (_jHistory == null)
                throw new ArgumentNullException(nameof(_jHistory));

            var history = new History
            {
                Date = _jHistory.GetValue("date").Value<DateTime>(),
                Title = _jHistory.GetValue("title").Value<string>(),
                Url = _jHistory.GetValue("url").Value<string>()
            };

            return history;
        }
    }
}
