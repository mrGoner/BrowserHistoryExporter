namespace BrowserHistoryExportApi.Serializers.Interfaces
{
    public interface IModelSerializer
    {
        string Extention { get; }
        string Serialize(HistoryCollection _model);
        HistoryCollection Deserialize(string _data);
    }
}