using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    public interface IDbEntityImporter
    {
        void Import<T>(ICollection<T> entities) where T : TableEntity;
    }
}
