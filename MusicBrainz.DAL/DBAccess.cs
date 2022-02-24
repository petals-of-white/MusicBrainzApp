using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;
using MusicBrainz.Tools.Config;
using MusicBrainz.Tools.Logging;
using static MusicBrainz.DAL.DataReaderToEntityConverterOld;

namespace MusicBrainz.DAL
{
    public class DbAccess
    {
        // to be removed later !!!
        private readonly HashSet<Type> _allTypes = new()
        {
            typeof(Area),
            typeof(Artist),
            typeof(Recording),
            typeof(Label),
            typeof(Release),
            typeof(ReleaseGroup),
            typeof(Work),
            typeof(Url),
            typeof(Place),
        };

        private string _connectionString = ConfigHelper.GetConnectionString();
        private LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        /// <summary>
        /// Gets tables names and number of records within them.
        /// </summary>
        /// <returns>A list of DbTableInfo instances</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        public IList<ITableInfo> GetDbTablesInfo()
        {
            string sql = @"select
                          t.name TableName,
                          i.rows Records
                        from
                          sysobjects t,
                          sysindexes i
                        where
                          t.xtype = 'U'
                          and i.id = t.id
                          and i.indid in (0, 1)
                          and t.name not like 'sys%'
                        order by
                          TableName;";

            SqlDataReader? reader;

            IList<ITableInfo>? output = default;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();
                        output = reader.Select(TableInfoFromReader).ToList();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }

            return output;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        public int? GetNumberOfRows(string tableName)
        {
            int? rows = null;

            string sql = @$"USE MusicBrainz;
                            SELECT COUNT(*)
                            FROM {tableName};";

            var queryResult = GetQueryResult(sql);

            try
            {
                object? rawData = queryResult.Rows [0].ItemArray [0];

                rows = Convert.ToInt32(rawData);
            }
            catch (DeletedRowInaccessibleException ex)
            {
                // log here
                _logger.Log(ex.ToString());
            }
            catch (IndexOutOfRangeException ex)
            {
                // log here
                _logger.Log(ex.ToString());
            }
            catch (FormatException ex)
            {
                _logger.Log(ex.ToString());
            }

            return rows;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        /// /// <exception cref="Exception"></exception>
        public T? GetRecordById<T>(int id) where T : TableEntity
        {
            T? output = default;

            Type entityType = typeof(T);

            SqlDataReader? reader;

            string entityTypeName = entityType.Name;

            string sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {entityTypeName}
                            WHERE Id = {id};";

            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new())
                    {
                        command.Connection = connection;

                        command.CommandText = sql;

                        reader = command.ExecuteReader();

                        if (reader.Read())

                        {
                            output = reader.Select<T>(GetRecordById).FirstOrDefault();
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.Log(ex.ToString());
            }

            return output;
        }

        /// <summary>
        /// Gets a record by id in the db, in the table specified tableOption
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tableOption"></param>
        /// <returns>An entity-object</returns>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public object? GetRecordById(int id, Tables tableOption)
        {
            string sql = @$"USE MusicBrainz;
                            SELECT *
                            FROM {tableOption}
                            WHERE Id = {id}";

            SqlDataReader? reader;

            object? output = default;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();

                        switch (tableOption)
                        {
                            case Tables.Area:
                                output = reader.Select<Area>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Artist:
                                output = reader.Select<Artist>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Label:
                                output = reader.Select<Label>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Place:
                                output = reader.Select<Place>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Recording:
                                output = reader.Select<Recording>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Release:
                                output = reader.Select<Release>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.ReleaseGroup:
                                output = reader.Select<ReleaseGroup>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Url:
                                output = reader.Select<Url>(GetRecordById).FirstOrDefault();
                                break;

                            case Tables.Work:
                                output = reader.Select<Work>(GetRecordById).FirstOrDefault();
                                break;
                        }

                        return output;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="tableOption"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<object?> GetRecordsList(int [] ids, Tables tableOption)
        {
            string sql = @$"USE MusicBrainz;
                            SELECT *
                            FROM {tableOption}
                            WHERE Id in (";

            StringBuilder sqlBuilder = new(sql);

            sqlBuilder.AppendJoin(", ", ids);

            sqlBuilder.Append(");");

            sql = sqlBuilder.ToString();

            SqlDataReader? reader;

            IEnumerable<object?> output = default;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();

                        switch (tableOption)
                        {
                            case Tables.Area:
                                output = reader.Select(AreaFromReader);
                                break;

                            case Tables.Artist:
                                output = reader.Select(ArtistFromReader, GetRecordById);
                                break;

                            case Tables.Label:
                                output = reader.Select(LabelFromReader, GetRecordById);
                                break;

                            case Tables.Place:
                                output = reader.Select(PlaceFromReader, GetRecordById);
                                break;

                            case Tables.Recording:
                                output = reader.Select(RecordingFromReader);
                                break;

                            case Tables.Release:
                                output = reader.Select(ReleaseFromReader, GetRecordById);
                                break;

                            case Tables.ReleaseGroup:
                                output = reader.Select(ReleaseGroupFromReader);
                                break;

                            case Tables.Url:
                                output = reader.Select(UrlFromReader);
                                break;

                            case Tables.Work:
                                output = reader.Select(WorkFromReader);
                                break;
                        }
                        output = output.ToArray();
                        return output;
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
        }

        public ICollection<T> GetTableRecords<T>(int? recordsPerPage = null, int? pageNumber = null) where T : TableEntity
        {
            Type table = typeof(T);

            List<T> output = default;

            object rawOutput = default;

            string sql;
            SqlDataReader? reader;

            if (recordsPerPage is null || pageNumber is null)
            {
                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {table.Name};";
            }
            else
            {
                if (recordsPerPage < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(recordsPerPage), recordsPerPage, "You can not have negative records per page");
                }

                if (pageNumber < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "You can not have negative page number");
                }

                int skippedRecords = recordsPerPage.Value * (pageNumber.Value - 1);

                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {table.Name}
                            ORDER BY Id
                            OFFSET {skippedRecords} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY;";
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();

                        if (table == typeof(Area))
                        {
                            rawOutput = reader.Select(AreaFromReader);
                        }
                        else if (table == typeof(Artist))
                        {
                            rawOutput = reader.Select(ArtistFromReader, GetRecordById);
                        }
                        else if (table == typeof(Label))
                        {
                            rawOutput = reader.Select(LabelFromReader, GetRecordById);
                        }
                        else if (table == typeof(Place))
                        {
                            rawOutput = reader.Select(PlaceFromReader, GetRecordById);
                        }
                        else if (table == typeof(Recording))
                        {
                            rawOutput = reader.Select(RecordingFromReader);
                        }
                        else if (table == typeof(Release))
                        {
                            rawOutput = reader.Select(ReleaseFromReader, GetRecordById);
                        }
                        else if (table == typeof(ReleaseGroup))
                        {
                            rawOutput = reader.Select(ReleaseGroupFromReader);
                        }
                        else if (table == typeof(Url))
                        {
                            rawOutput = reader.Select(UrlFromReader);
                        }
                        else if (table == typeof(Work))
                        {
                            rawOutput = reader.Select(WorkFromReader);
                        }

                        output = ((IEnumerable<T>) rawOutput!).ToList();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }

            return output;
        }

        public ICollection<T> GetTableRecordsGenericMapProperties<T>(int? recordsPerPage = null, int? pageNumber = null) where T : TableEntity
        {
            Type table = typeof(T);

            List<T> output = default;

            string sql;
            SqlDataReader? reader;

            if (recordsPerPage is null || pageNumber is null)
            {
                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {table.Name};";
            }
            else
            {
                if (recordsPerPage < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(recordsPerPage), recordsPerPage, "You can not have negative records per page");
                }

                if (pageNumber < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "You can not have negative page number");
                }

                int skippedRecords = recordsPerPage.Value * (pageNumber.Value - 1);

                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {table.Name}
                            ORDER BY Id
                            OFFSET {skippedRecords} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY;";
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();

                        output = reader.Select<T>(GetRecordById).ToList();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }

            return output;
        }

        /// <summary>
        /// Gets table records from the db according to Table option,
        /// also optionally pagination
        /// </summary>
        /// <param name="tableOption"></param>
        /// <param name="recordsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <returns>An IEnumerable of entities objects</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ICollection<object> GetTableRecordsOldNoGeneric(Tables tableOption, int? recordsPerPage = null, int? pageNumber = null)
        {
            SqlDataReader? reader;
            string sql;
            IEnumerable<object> enumerableRawResult = default;
            ICollection<object> output = default;

            if (recordsPerPage is null || pageNumber is null)
            {
                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {tableOption};";
            }
            else
            {
                if (recordsPerPage < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(recordsPerPage), recordsPerPage, "You can not have negative records per page");
                }

                if (pageNumber < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "You can not have negative page number");
                }

                int skippedRecords = recordsPerPage.Value * (pageNumber.Value - 1);

                sql = @$"USE MusicBrainz;
                            SELECT
                               *
                            FROM
                               {tableOption}
                            ORDER BY Id
                            OFFSET {skippedRecords} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY;";
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = sql;

                        reader = cmd.ExecuteReader();

                        switch (tableOption)
                        {
                            case Tables.Area:
                                enumerableRawResult = reader.Select(AreaFromReader);
                                break;

                            case Tables.Artist:
                                enumerableRawResult = reader.Select(ArtistFromReader, GetRecordById);
                                break;

                            case Tables.Label:
                                enumerableRawResult = reader.Select(LabelFromReader, GetRecordById);
                                break;

                            case Tables.Place:
                                enumerableRawResult = reader.Select(PlaceFromReader, GetRecordById);
                                break;

                            case Tables.Recording:
                                enumerableRawResult = reader.Select(RecordingFromReader);
                                break;

                            case Tables.Release:
                                enumerableRawResult = reader.Select(ReleaseFromReader, GetRecordById);
                                break;

                            case Tables.ReleaseGroup:
                                enumerableRawResult = reader.Select(ReleaseGroupFromReader);
                                break;

                            case Tables.Url:
                                enumerableRawResult = reader.Select(UrlFromReader);
                                break;

                            case Tables.Work:
                                enumerableRawResult = reader.Select(WorkFromReader);
                                break;
                        }
                        output = enumerableRawResult!.ToList();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }

            return output;
        }

        /// <summary>
        /// Inserts a new records to a correspoding table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void InsertEntities<T>(ICollection<T> entities) where T : TableEntity
        {
            Type entityType = typeof(T);
            DynamicParameters entityParams;

            string storedProcedureName = $"spInsert{entityType.Name}";

            using (SqlConnection connection = new(_connectionString))
            {
                foreach (T entity in entities)
                {
                    entityParams = new DynamicParameters();
                    foreach (var property in entityType.GetProperties().Where(x => x.Name != "Id"))
                    {
                        object? valueToAdd = typeof(TableEntity).IsAssignableFrom(property.PropertyType)
                            ? (property.GetValue(entity) as TableEntity)?.Id
                            : property.GetValue(entity);

                        entityParams.Add(property.Name, valueToAdd);
                    }

                    // executing stored procedure

                    int affectedRows = connection.Execute(sql: storedProcedureName, param: entityParams, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public void InsertEntities(Tables table, IEnumerable<object> entities)
        {
            using SqlConnection connection = new();

            string storedProcedure = $"";
            connection.Execute("", commandType: CommandType.StoredProcedure);

            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        ///
        private DataTable GetQueryResult(string sql)
        {
            DataTable output = new();
            SqlDataReader? reader;

            try
            {
                using var connection = new SqlConnection(_connectionString);

                connection.Open();

                using SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = sql;

                reader = cmd.ExecuteReader();

                output = reader.GetSchemaTable();

                output.Load(reader);

                return output;
            }
            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
        }
    }
}