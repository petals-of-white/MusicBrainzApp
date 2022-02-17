namespace MusicBrainz.BLL.Exporting
{
    public interface ITableExporterBuilder : ITableUsable
    {
        ITableExporter Build();
    }

}
