namespace BrowserHistoryExportApi.Serializers.Interfaces
{
    public interface IModelSerializer
    {
        string Serialize(HistoryCollection _model);
        HistoryCollection Deserialize(string _data);
    }
}