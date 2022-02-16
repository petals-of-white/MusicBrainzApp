using MusicBrainz.BLL.DbEntitySerialization.DataTransfer;
using MusicBrainz.BLL.DbEntitySerialization.Serialization;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.BLL.DbEntitySerialization
{
    public class DbEntitiesSerializer
    {
        private readonly DbEntityImporterExporter _entityImporterExporter = new();

        private readonly ISerializationManager _serializationManager = new JsonSerializationManager();

        private DbExportImportConfig _config;

        public DbEntitiesSerializer(DbExportImportConfig config)
        {
            _config = config;
        }

        public void SerializeTableEntities()
        {
            foreach (var table in _config.TablesToExport)
            {
                // get entities list
                var entities = _entityImporterExporter.Export(table, _config.RecordsPerPage, _config.PageNumber);

                //serialize them to json
                string serializedEntities = _serializationManager.Serialize(entities);

                var directory = Directory.CreateDirectory("export").Name;
                var fileName = $"{table}.json";
                string fullPath = Path.Combine(directory, fileName);
                File.WriteAllText(fullPath, serializedEntities);
            }
        }

        public IList<ITableInfo> GetTablesInfo()
        {
            return _entityImporterExporter.GetTablesInfo();
        }

        public void ImportSerializedTableEntities()
        {
            throw new NotImplementedException();
        }
    }

}