using HelperLibrary;
using HelperLibrary.Logging;
using Microsoft.Data.SqlClient;
using MusicBrainzDataAcessLibrary;
using MusicBrainzModelsLibrary.Entities;
using MusicBrainzModelsLibrary.Tables;
using Newtonsoft.Json;
namespace MusicBrainzExportLibrary.Exporting
{
    public class TableToJsonExporter : IPaginatedTableExporter
    {
        private LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        private DBAccess _db = new();

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="UserFriendlyException"></exception>
        public TableToJsonExporter()
        {
            try
            {
                TablesInfo = _db.GetTablesInfo();
            }

            catch (Exception ex)
            {
                throw new UserFriendlyException("An error has occured while trying to get data from the database. Please try again. Later", ex);
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
                throw new UserFriendlyException($"The database doesn't have a table named {table.Name}", ex);
                //throw new ArgumentException($"The database doesn't have a table named {table.Name}", ex);
            }

            try
            {
                switch (tableOption)
                {
                    case AvailableTables.Area:
                        entities = _db.GetTableRecords<Area>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Artist:
                        entities = _db.GetTableRecords<Artist>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Label:
                        entities = _db.GetTableRecords<Label>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Place:
                        entities = _db.GetTableRecords<Place>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Recording:
                        entities = _db.GetTableRecords<Recording>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Release:
                        entities = _db.GetTableRecords<Release>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.ReleaseGroup:
                        entities = _db.GetTableRecords<ReleaseGroup>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Url:
                        entities = _db.GetTableRecords<Url>(RecordsPerPage, PageNumber);
                        break;

                    case AvailableTables.Work:
                        entities = _db.GetTableRecords<Work>(RecordsPerPage, PageNumber);
                        break;
                };
            }

            catch (ArgumentOutOfRangeException ex)
            {
                _logger.Log(ex.ToString());
                throw new UserFriendlyException(ex.Message, ex);
            }

            return entities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="UserFriendlyException"></exception>
        ///// <exception cref="IOException"></exception>
        ///// <exception cref="UnauthorizedAccessException"></exception>
        ///// <exception cref="ArgumentOutOfRangeException"></exception>
        ///// <exception cref="ArgumentException"></exception>
        ///// <exception cref="SqlException"></exception>
        public void Export()
        {
            foreach (ITable table in SelectedTables)
            {
                string tableJsonPath = $"{table.Name}.json";
                IEnumerable<object>? rawRecords = null;

                try
                {
                    rawRecords = getEntitiesListFromDb(table);

                }

                catch (UserFriendlyException ex)
                {
                    _logger.Log(ex.ToString());
                }

                //catch (ArgumentOutOfRangeException ex)
                //{
                //    throw;
                //}

                //catch (ArgumentException ex)
                //{
                //    throw;
                //}

                //catch (SqlException ex)
                //{
                //    throw;
                //}

                string json = JsonConvert.SerializeObject(rawRecords, Formatting.Indented);

                try
                {
                    File.WriteAllText(tableJsonPath, string.Empty);
                    File.AppendAllText(tableJsonPath, json);
                }

                catch (Exception ex)
                {
                    throw new UserFriendlyException("An error has occured while trying to write data to a json file. Please try again later.", ex);
                }
                //catch (IOException ex)
                //{
                //    throw;
                //}

                //catch (UnauthorizedAccessException ex)
                //{
                //    throw;
                //}
            }
        }

        public void Import()
        {
            throw new NotImplementedException();
        }

    }

}
