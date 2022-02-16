using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    public interface IDbEntityExporter
    {
        IEnumerable<object> Export(Tables table, int? recordsPerPage, int? pageNumber);
    }
}