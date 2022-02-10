using HelperLibrary;
using Microsoft.Data.SqlClient;
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

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SqlException"></exception>
        public TableToJsonExporter()
        {
            try
            {
                TablesInfo = _db.GetTablesInfo();
            }
            catch (SqlException ex)
            {
                throw;
            }
        }

        public string JsonPath { get; internal set; } = ConfigHelper.Configuration.GetSection("JsonPath").GetSection("Default").Value;
        public int? PageNumber { get; internal set; }

        public bool PaginationEnabled { get; internal set; } = false;

        public int? RecordsPerPage { get; internal set; }

        public IList<ITable> SelectedTables { get; internal set; } = new List<ITable>();

        public IList<ITable> TablesInfo { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SqlException"></exception>
        private IEnumerable<object>? getEntitiesListFromDb(ITable table)
        {
            IEnumerable<object>? entities = null;

            object tableOption;

            try
            {
                tableOption = Enum.Parse(typeof(AvailableTables), table.Name);
            }

            catch (ArgumentException ex)
            {
                throw;
            }

            try
            {
                switch (PaginationEnabled)
                {
                    case true:

                        switch (tableOption)
                        {
                            case AvailableTables.Area:
                                entities = _db.GetTableRecords<Area>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Artist:
                                entities = _db.GetTableRecords<Artist>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Label:
                                entities = _db.GetTableRecords<Label>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Place:
                                entities = _db.GetTableRecords<Place>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Recording:
                                entities = _db.GetTableRecords<Recording>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Release:
                                entities = _db.GetTableRecords<Release>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.ReleaseGroup:
                                entities = _db.GetTableRecords<ReleaseGroup>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Url:
                                entities = _db.GetTableRecords<Url>(RecordsPerPage.Value, PageNumber.Value);
                                break;

                            case AvailableTables.Work:
                                entities = _db.GetTableRecords<Work>(RecordsPerPage.Value, PageNumber.Value);
                                break;
                        }

                        break;

                    case false:

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

                        break;
                }
            }

            catch (ArgumentOutOfRangeException ex)
            {
                throw;
            }

            catch (ArgumentException ex)
            {
                throw;
            }

            catch (SqlException ex)
            {
                throw;
            }

            return entities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        public void Export()
        {
            //File.WriteAllText(JsonPath, string.Empty);

            foreach (ITable table in SelectedTables)
            {
                string tableJsonPath = $"{table.Name}.json";
                try
                {
                    File.WriteAllText(tableJsonPath, string.Empty);

                }
                catch (IOException ex)
                {
                    throw;
                }

                catch (UnauthorizedAccessException ex)
                {
                    throw;
                }

                IEnumerable<object>? rawRecords;

                try
                {
                    rawRecords = getEntitiesListFromDb(table);

                }

                catch (ArgumentOutOfRangeException ex)
                {
                    throw;
                }

                catch (ArgumentException ex)
                {
                    throw;
                }

                catch (SqlException ex)
                {
                    throw;
                }

                string json = JsonConvert.SerializeObject(rawRecords, Formatting.Indented);

                try
                {
                    File.AppendAllText(tableJsonPath, json);

                }
                catch (IOException ex)
                {
                    throw;
                }

                catch (UnauthorizedAccessException ex)
                {
                    throw;
                }

            }
        }

        public void Import()
        {
            throw new NotImplementedException();
        }

    }

}
