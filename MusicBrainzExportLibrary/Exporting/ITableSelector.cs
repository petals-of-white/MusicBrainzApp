using MusicBrainzModelsLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface ITableSelector
    {
        IList<ITable> TablesInfo { get; }
        IList<ITable> SelectedTables { get; }
    }

}
