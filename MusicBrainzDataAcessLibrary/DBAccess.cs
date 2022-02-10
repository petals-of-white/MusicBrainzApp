using System.Data;
using HelperLibrary;
using Microsoft.Data.SqlClient;
using MusicBrainzModelsLibrary.Entities;
using MusicBrainzModelsLibrary.Tables;

namespace MusicBrainzDataAcessLibrary
{
    public class DBAccess : DBAccessBase
    {


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
        public int GetNumberOfRows(string tableName)
        {
            int rows = -1;
            string sql = @$"SELECT COUNT(*)
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
                throw;
            }
            catch (IndexOutOfRangeException ex)
            {
                // log here
                _logger.Log(ex.ToString());
                throw;
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

            try
            {
                tablesInfo = GetQueryResult(sql);
            }

            catch (ArgumentException)
            {
                throw;
            }

            catch (SqlException)
            {
                throw;
            }


            DataTable tableInfoWithNumberOfRows = tablesInfo.Clone();


            try
            {
                tableInfoWithNumberOfRows.Columns.Add("NumberOfRecords", typeof(string));

            }
            catch (InvalidExpressionException ex)
            {
                throw;
            }


            foreach (DataRow row in tablesInfo.Rows)
            {
                int tableNumber = Convert.ToInt32(row [0]);
                string tableName = (string) row [1];
                int numberOfRows = GetNumberOfRows(tableName);

                tableInfoWithNumberOfRows.Rows.Add(tableNumber, tableName, numberOfRows);
            }

            IList<ITable> outputList = new List<ITable>();

            foreach (DataRow row in tableInfoWithNumberOfRows.Rows)
            {
                try
                {
                    outputList.Add(row.ToObject<Table>());

                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return outputList;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<object> GetTableRecords(string tableName)
        {
            throw new NotImplementedException();
            //DataTable output;
            //string sql = @$"USE MusicBrainz;
            //                SELECT
            //                   * 
            //                FROM
            //                   {tableName};";

            //try
            //{
            //    output = GetQueryResult(sql);

            //}

            //catch (ArgumentException ex)
            //{
            //    throw;
            //}

            //catch (SqlException ex)
            //{
            //    throw;
            //}

            //List<object> records = new List<object>();

            //foreach (DataRow row in output.Rows)
            //{
            //    try
            //    {
            //        row.ToObject<Area>();
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
            //return new List<object>();

        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        /// /// <exception cref="Exception"></exception>
        public T GetRecordById<T>(int id) where T : new()
        {
            Type entityType = typeof(T);

            T? result;
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
            catch (ArgumentException)
            {
                throw;
            }

            catch (SqlException)
            {
                throw;
            }

            catch (Exception e)
            {
                throw;
            }

            return result;
        }



        /// <summary>
        /// Executes sql and returns a list of entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private IEnumerable<T> TableRecordsHelper<T>(int? recordsPerPage = null, int? pageNumber = null) where T : new()
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

            try
            {
                originalOutput = GetQueryResult(sql);

                // a structure copy of original table
                mappedOutput = originalOutput.Clone();

                //deleting gid
                mappedOutput.Columns.Remove("gid");
            }

            catch (ArgumentException ex)
            {
                throw;
            }

            catch (SqlException ex)
            {
                throw;
            }


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
                                //foreignRecord = GetQueryResult(sql2).Rows [0].ToObject<Area>();
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
                try
                {
                    entitiesList.Add(mappedRow.ToObject<T>());

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return entitiesList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SqlException"></exception>
        public IEnumerable<T> GetTableRecords<T>(int recordsPerPage, int pageNumber) where T : new()
        {
            IEnumerable<T> entitiesList;

            try
            {
                entitiesList = TableRecordsHelper<T>(recordsPerPage, pageNumber);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }

            catch (SqlException)
            {
                throw;
            }

            return entitiesList;
        }



        /// <summary>
        /// Returns IEnumerable<T> where T is a record entity in a table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<T> GetTableRecords<T>() where T : new()
        {
            IEnumerable<T> entitiesList;
            try
            {
                entitiesList = TableRecordsHelper<T>();
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


            return entitiesList;
        }

    }

}
