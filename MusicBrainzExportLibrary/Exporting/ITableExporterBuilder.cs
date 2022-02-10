namespace MusicBrainzExportLibrary.Exporting
{
    public interface ITableExporterBuilder : ITableUsable
    {
        ITableExporter Build();
    }

}
