using MusicBrainz.BLL.DbEntitySerialization.DataTransfer;
using MusicBrainz.BLL.DbEntitySerialization.Serialization;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.BLL.DbEntitySerialization
{
    public class DbEntitiesSerializer
    {
        private readonly DbEntityImporterExporter _entityImporterExporter = new();

        private readonly ISerializationManager _serializationManager = new JsonSerializationManager();

        private DbExportImportConfig? _config;

        private DbImportConfig _importConfig;


        private DbExportConfig _exportConfig;

        public DbEntitiesSerializer(DbExportImportConfig config)
        {
            _config = config;
        }

        public DbEntitiesSerializer()
        {
        }

        public DbEntitiesSerializer(DbImportConfig importConfig, DbExportConfig exportConfig)
        {
            _importConfig = importConfig;
            _exportConfig = exportConfig;
        }

        public DbEntitiesSerializer(ISerializationManager serializationManager)
        {
            _serializationManager = serializationManager;
        }

        public void ConfigureExport(DbExportConfig exportConfig)
        {
            _exportConfig = exportConfig;

        }
        public void ConfigureImport(DbImportConfig importConfig)
        {
            _importConfig = importConfig;
        }

        public Dictionary<Tables, string> SerializeTabelEntitiesTypeMapped()
        {
            Dictionary<Tables, string> serializedOutput = new();
            string serializedEntities;

            int? recordsPerPage = _exportConfig.RecordsPerPage;
            int? pageNumber = _exportConfig.PageNumber;

            foreach (var table in _exportConfig.TablesToExport)
            {
                ICollection<TableEntity> entities = new List<TableEntity>();

                switch (table)
                {
                    case Tables.Area:
                        entities = _entityImporterExporter.Export<Area>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Artist:
                        entities = _entityImporterExporter.Export<Artist>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Label:
                        entities = _entityImporterExporter.Export<Label>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Place:
                        entities = _entityImporterExporter.Export<Place>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Recording:
                        entities = _entityImporterExporter.Export<Recording>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList(); ;
                        break;

                    case Tables.Release:
                        entities = _entityImporterExporter.Export<Release>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.ReleaseGroup:
                        entities = _entityImporterExporter.Export<ReleaseGroup>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Work:
                        entities = _entityImporterExporter.Export<Work>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;

                    case Tables.Url:
                        entities = _entityImporterExporter.Export<Url>(recordsPerPage, pageNumber).Cast<TableEntity>().ToList();
                        break;
                }

                //serialize them to json
                serializedEntities = _serializationManager.Serialize(entities);

                serializedOutput.Add(table, serializedEntities);
            }
            return serializedOutput;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A Dictionary with Table enum as key and serialized entities as values</returns>
        public Dictionary<Tables, string> SerializeTableEntities()
        {
            Dictionary<Tables, string> serializedOutput = new();

            foreach (var table in _exportConfig.TablesToExport)
            {
                // get entities list
                var entities = _entityImporterExporter.Export(table, _exportConfig.RecordsPerPage, _exportConfig.PageNumber);

                //serialize them to json
                string serializedEntities = _serializationManager.Serialize(entities);

                serializedOutput.Add(table, serializedEntities);

                //var directory = Directory.CreateDirectory("export").Name;
                //var fileName = $"{table}.json";
                //string fullPath = Path.Combine(directory, fileName);

                //File.WriteAllText(fullPath, serializedEntities);
            }
            return serializedOutput;
        }

        public IList<ITableInfo> GetTablesInfo()
        {
            return _entityImporterExporter.GetTablesInfo();
        }

        public void ImportSerializedTableEntities()
        {
            foreach (var importableTableEntities in _importConfig.EntitiesToImport)
            {
                switch (importableTableEntities.Key)
                {
                    case Tables.Area:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Area>().ToList());
                        break;

                    case Tables.Artist:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Artist>().ToList());
                        break;

                    case Tables.Label:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Label>().ToList());
                        break;

                    case Tables.Recording:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Recording>().ToList());
                        break;

                    case Tables.Place:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Place>().ToList());
                        break;

                    case Tables.Release:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Release>().ToList());
                        break;

                    case Tables.ReleaseGroup:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<ReleaseGroup>().ToList());
                        break;

                    case Tables.Work:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Work>().ToList());
                        break;

                    case Tables.Url:
                        _entityImporterExporter.Import(importableTableEntities.Value.Cast<Url>().ToList());
                        break;
                }
            }
        }

        public void ImportSerializedTableEntitiesNew()
        {

            foreach (var importableTableEntities in _importConfig.SerializedEntitiesToImport)
            {
                switch (importableTableEntities.Key)
                {
                    case Tables.Area:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Area>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Artist:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Artist>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Label:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Label>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Recording:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Recording>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Place:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Place>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Release:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Release>>(importableTableEntities.Value)!);
                        break;

                    case Tables.ReleaseGroup:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<ReleaseGroup>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Work:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Work>>(importableTableEntities.Value)!);
                        break;

                    case Tables.Url:
                        _entityImporterExporter.Import(
                            _serializationManager
                            .Deserialize<List<Url>>(importableTableEntities.Value)!);
                        break;
                }
            }
        }

    }
}