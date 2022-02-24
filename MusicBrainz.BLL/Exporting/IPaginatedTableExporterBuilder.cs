namespace MusicBrainz.BLL.Exporting
{
    public interface IPaginatedTableExporterBuilder : ITableUsable, IPaginationCreator
    {
        IPaginatedTableExporter Build();
    }
}