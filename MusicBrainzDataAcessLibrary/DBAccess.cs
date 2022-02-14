using System.Data;
using HelperLibrary;
using HelperLibrary.Logging;
using Microsoft.Data.SqlClient;
using MusicBrainzModelsLibrary.Entities;
using MusicBrainzModelsLibrary.Tables;

namespace MusicBrainzDataAcessLibrary
{
    public class DBAccess
    {

        protected LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        protected string _connectionString = ConfigHelper.GetConnectionString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        protected DataTable GetQueryResult(string sql)
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
        public IList<ITable> GetTablesInfo()
        {
            string sql = @"USE MusicBrainz;
                            SELECT
                               # = ROW_NUMBER() OVER (
                            ORDER BY
                               t.TABLE_NAME),
                               t.TABLE_NAME as Name
                            FROM
                               INFORMATION_SCHEMA.TABLES t
                            WHERE
                               TABLE_TYPE = 'BASE TABLE'
                               AND t.TABLE_NAME != 'sysdiagrams'
                            Order by
                               t.TABLE_NAME;";

            DataTable tablesInfo;

            tablesInfo = GetQueryResult(sql);

            DataTable tableInfoWithNumberOfRows = tablesInfo.Clone();

            try
            {
                tableInfoWithNumberOfRows.Columns.Add("NumberOfRecords", typeof(string));
            }

            catch (DuplicateNameException ex)
            {
                _logger.Log(ex.ToString());
            }

            catch (InvalidExpressionException ex)
            {
                _logger.Log(ex.ToString());
            }

            foreach (DataRow row in tablesInfo.Rows)
            {
                int tableNumber = Convert.ToInt32(row [0]);
                string tableName = (string) row [1];
                int? numberOfRows = GetNumberOfRows(tableName);

                tableInfoWithNumberOfRows.Rows.Add(tableNumber, tableName, numberOfRows);
            }

            IList<ITable> outputList = new List<ITable>();

            foreach (DataRow row in tableInfoWithNumberOfRows.Rows)
            {
                // handle exceptions
                outputList.Add(row.ToObject<Table>());
            }
            return outputList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        /// /// <exception cref="Exception"></exception>
        public T? GetRecordById<T>(int id) where T : new()
        {
            Type entityType = typeof(T);

            T? result = default(T);
            string entityTypeName = entityType.Name;

            string sql = @$"USE MusicBrainz;
                            SELECT
                               * 
                            FROM
                               {entityTypeName}
                            WHERE Id = {id};";

            try
            {
                result = GetQueryResult(sql).Rows [0].ToObject<T>();
            }

            catch (IndexOutOfRangeException ex)
            {
                _logger.Log(ex.ToString());
            }


            return result;
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
            foreach (var entityProperty in entityType.GetProperties())
            {
                if (_allTypes.Contains(entityProperty.PropertyType))
                {
                    mappedOutput.Columns [entityProperty.Name].DataType = entityProperty.PropertyType;
                }
            }


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
