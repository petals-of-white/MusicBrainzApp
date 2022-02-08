using MusicBrainzModelsLibrary.Tables;


namespace MusicBrainzExportLibrary.Exporting
{
    public interface IPaginatedTableExporterBuilder:  ITableUsable, IPaginationCreator
    {
        IPaginatedTableExporter Build();

    }
}
