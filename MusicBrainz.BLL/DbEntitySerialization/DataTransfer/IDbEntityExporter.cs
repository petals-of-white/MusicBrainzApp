using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    public interface IDbEntityExporter
    {
        ICollection<object> Export(Tables table, int? recordsPerPage, int? pageNumber);
    }
}
