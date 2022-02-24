using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;
using MusicBrainz.Tools;
using MusicBrainz.Tools.Config;
using MusicBrainz.Tools.Logging;
using static MusicBrainz.DAL.DataReaderToEntityConverter;
namespace MusicBrainz.DAL
{
    public class DbAccess
    {
        private LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        private string _connectionString = ConfigHelper.GetConnectionString();

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
                        entityParams.Add(property.Name, property.GetValue(entity));
                    }


                    // executing stored procedure


                    int affectedRows = connection.Execute(sql: storedProcedureName, param: entityParams, commandType: CommandType.StoredProcedure);
                }
            }
        }

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
        /// Gets table records from the db according to Table option,
        /// also optionally pagination
        /// </summary>
        /// <param name="tableOption"></param>
        /// <param name="recordsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <returns>An IEnumerable of entities objects</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ICollection<object> GetTableRecords(Tables tableOption, int? recordsPerPage = null, int? pageNumber = null)
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

                        /*!!!!!!!!!!!!!experiment start
                        var outputCol = new List<List<int>>();

                        var testDel = delegate (IDataReader reader)
                        {
                            List<int> foreignKeys = new();

                            string [] typesNames = Enum.GetNames(typeof(Tables));

                            var fieldNames = Enumerable.Range(0, reader.FieldCount)
                                .Select(reader.GetName)
                                .ToArray();

                            foreach (string fieldName in fieldNames)
                            {
                                if (typesNames.Contains(fieldName))
                                {
                                    foreignKeys.Add((int) reader [fieldName]);
                                }
                            }
                            outputCol.Add(foreignKeys);
                        };


                        var test2 = delegate (IDataReader reader)
                        {
                            return new List<int?>(new int? [] { (int?) reader ["Area"], (int?) reader [""] });
                        };

                        
                         * !!!!!!!!!!!!experiment end
                         * 
                         */

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

        public ICollection<T> GetTableRecordsGenericMapDefault<T>(int? recordsPerPage = null, int? pageNumber = null) where T : TableEntity
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
        /// Gets table records from the db according to T entity.
        /// </summary>
        /// <returns>A list of T entities.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when recordsPerPage or pageNumber is les than 1</exception>
        /// <exception cref="SqlException">DB problem</exception>
        // This method needs a better implementing
        public IEnumerable<T> GetTableRecords<T>(int? recordsPerPage = null, int? pageNumber = null) where T : new()
        {
            string sql;

            Type entityType = typeof(T);

            string entityTypeName = entityType.Name;


            if (recordsPerPage is null || pageNumber is null)
            {
                sql = @$"USE MusicBrainz;
                            SELECT
                               * 
                            FROM
                               {entityTypeName};";
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
                               {entityTypeName}
                            ORDER BY Id
                            OFFSET {skippedRecords} ROWS FETCH NEXT {recordsPerPage} ROWS ONLY;";

            }

            List<T> entitiesList = new(); // this will be returned

            DataTable originalOutput,
            mappedOutput;

            originalOutput = GetQueryResult(sql);

            // a structure copy of original table
            mappedOutput = originalOutput.Clone();

            try
            {
                //deleting gid
                mappedOutput.Columns.Remove("gid");
            }

            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
            }

            // !!!! Create a separate entities helper !!!!
            // changing column type to corresponding entity

            //NewMethod(entityType, mappedOutput);


            // filling mappedOutput, using Entities instead of foreign keys.

            foreach (DataRow originalRow in originalOutput.Rows)
            {
                // new row created
                DataRow mappedRow = mappedOutput.NewRow();

                // checking all the properties of the entity
                foreach (var entityProperty in entityType.GetProperties())
                {
                    // vars for convenience
                    Type propertyType = entityProperty.PropertyType;
                    string propertyName = entityProperty.Name;

                    // if property's type is an Entity Class...
                    if (_allTypes.Contains(propertyType))
                    {
                        // getting a foreign key
                        int? foreignKey = Convert.IsDBNull(originalRow [propertyName]) ? null : (int) originalRow [propertyName];

                        object? foreignRecord = null;

                        if (foreignKey != null)
                        {
                            if (propertyType == typeof(Area))
                            {
                                // GetRecordsById - get list instead of one record. !!!!!!!!!!!!!!!!
                                foreignRecord = GetRecordById<Area>(foreignKey.Value);
                            }

                            else if (propertyType == typeof(ReleaseGroup))
                            {
                                //foreignRecord = GetQueryResult(sql2).Rows [0].ToObject<ReleaseGroup>();
                                foreignRecord = GetRecordById<ReleaseGroup>(foreignKey.Value);
                            }
                        }
                        mappedRow [propertyName] = foreignRecord;
                    }

                    else
                    {
                        mappedRow [propertyName] = originalRow [propertyName];
                    }
                }

                // finally, adding a new row
                mappedOutput.Rows.Add(mappedRow);
            }

            foreach (DataRow mappedRow in mappedOutput.Rows)
            {
                entitiesList.Add(mappedRow.ToObject<T>());
            }

            return entitiesList;
        }



    }
}
