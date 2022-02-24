using MusicBrainz.Common.TableModels;

namespace MusicBrainz.BLL.Exporting
{
    public interface ITableSelector
    {
        IList<DbTableInfo> TablesInfo { get; }
        IList<DbTableInfo> SelectedTables { get; }
    }
}