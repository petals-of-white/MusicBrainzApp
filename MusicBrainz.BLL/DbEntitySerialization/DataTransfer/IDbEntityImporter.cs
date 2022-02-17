using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    public interface IDbEntityImporter
    {
        void Import(Tables table, IEnumerable<object> entities);
    }
}