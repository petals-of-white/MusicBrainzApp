using MusicBrainz.Common.TableModels;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    public interface IDbTablesInfoGetter
    {
        IList<ITableInfo> GetTablesInfo();
    }
}
