using MusicBrainzExportLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface ITableExporterBuilder
    {
        void UseTable(ITable table);
        void UseAllTables();
        void EnablePagination(int recordsPerPage, int pageNumber);
        void DisablePagination();
        ITableExporter Build();

    }
}
