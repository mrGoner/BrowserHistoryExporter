namespace BrowserHistoryExportApi.Serializers.Interfaces
{
    public interface IModelSerializer
    {
        string Serialize(ExportModel _model);
        ExportModel Deserialize(string _data);
    }
}