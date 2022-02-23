using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization
{
    public class DbImportConfig
    {
        public Dictionary<Tables, ICollection<TableEntity>> EntitiesToImport { get; private set; } =
                   new Dictionary<Tables, ICollection<TableEntity>>();

        public Dictionary<Tables, string> SerializedEntitiesToImport { get; private set; } =
                   new Dictionary<Tables, string>();

        public void AddEntitiesToImport(Tables table, ICollection<TableEntity> entities)
        {
            EntitiesToImport.Add(table, entities);
        }

        public void AddEntitiesToImport(Tables table, string serializedEntitiesList)
        {
            SerializedEntitiesToImport.Add(table, serializedEntitiesList);
        }
    }
}