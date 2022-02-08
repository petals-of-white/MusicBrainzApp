using HelperLibrary;
using MusicBrainzDataAcessLibrary;
using MusicBrainzModelsLibrary.Entities;
using MusicBrainzModelsLibrary.Tables;
using Newtonsoft.Json;
namespace MusicBrainzExportLibrary.Exporting
{
    public class TableToJsonExporter : IPaginatedTableExporter
    {

        //public IDictionary<int, ITable> TablesInfo { get; internal set; }

        private DBAccess _db = new();

        public TableToJsonExporter()
        {
            TablesInfo = _db.GetTablesInfo();
        }

        public string JsonPath { get; internal set; } = ConfigHelper.Configuration.GetSection("JsonPath").GetSection("Default").Value;
        public int? PageNumber { get; internal set; }

        public bool PaginationEnabled { get; internal set; } = false;

        public int? RecordsPerPage { get; internal set; }

        public IList<ITable> SelectedTables { get; internal set; } = new List<ITable>();

        public IList<ITable> TablesInfo { get; internal set; }

        private IEnumerable<object>? getEntitiesListFromDb (ITable table)
        {
            IEnumerable<object>? entities = null;

            var tableOption = Enum.Parse(typeof(AvailableTables), table.Name);

            switch (tableOption)
            {
                case AvailableTables.Area:
                    entities = _db.GetTableRecords<Area>();
                    break;

                case AvailableTables.Artist:
                    entities = _db.GetTableRecords<Artist>();
                    break;

                case AvailableTables.Label:
                    entities = _db.GetTableRecords<Label>();
                    break;

                case AvailableTables.Place:
                    entities = _db.GetTableRecords<Place>();
                    break;

                case AvailableTables.Recording:
                    entities = _db.GetTableRecords<Recording>();
                    break;

                case AvailableTables.Release:
                    entities = _db.GetTableRecords<Release>();
                    break;

                case AvailableTables.ReleaseGroup:
                    entities = _db.GetTableRecords<ReleaseGroup>();
                    break;

                case AvailableTables.Url:
                    entities = _db.GetTableRecords<Url>();
                    break;

                case AvailableTables.Work:
                    entities = _db.GetTableRecords<Work>();
                    break;
            }
            return entities;
        }

        public void Export()
        {
            //File.WriteAllText(JsonPath, string.Empty);

            foreach (ITable table in SelectedTables)
            {
                string tableJsonPath = $"{table.Name}.json";
                File.WriteAllText(tableJsonPath, string.Empty);

                IEnumerable<object>? rawRecords = getEntitiesListFromDb(table);

                string json = JsonConvert.SerializeObject(rawRecords, Formatting.Indented);

                //File.AppendAllText(JsonPath, json);
                File.AppendAllText(tableJsonPath, json);
            }
        }

        public void Import()
        {
            throw new NotImplementedException();
        }

    }

}
