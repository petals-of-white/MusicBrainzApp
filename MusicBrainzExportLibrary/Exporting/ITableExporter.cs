using MusicBrainzExportLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface ITableExporter
    {
        IDictionary<int, ITable> TablesInfo { get; }
        IList<ITable> SelectedTables { get; }
        int? PageNumber { get; }
        bool PaginationEnabled { get; }
        int? RecordsPerPage { get; }
        void Export();
    }
}