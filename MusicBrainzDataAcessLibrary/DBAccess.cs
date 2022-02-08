using System.Data;
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

            catch (DeletedRowInaccessibleException)
            {
                // log here
                throw;
            }
            catch (IndexOutOfRangeException)
            {
                // log here
                throw;
            }

            return rows;

        }


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

            DataTable tablesInfo = GetQueryResult(sql);

            DataTable tableInfoWithNumberOfRows = tablesInfo.Clone();

            tableInfoWithNumberOfRows.Columns.Add("NumberOfRecords", typeof(string));


            foreach (DataRow row in tablesInfo.Rows)
            {
                int tableNumber = Convert.ToInt32( row[0]);
                string tableName = (string) row [1];
                int numberOfRows = GetNumberOfRows(tableName);

                tableInfoWithNumberOfRows.Rows.Add(tableNumber, tableName, numberOfRows);
            }

            IList<ITable> outputList = new List<ITable>();

            foreach(DataRow row in tableInfoWithNumberOfRows.Rows)
            {
                outputList.Add(row.ToObject<Table>());
            }

            return outputList;

        }

        public IEnumerable<object> GetTableRecords(string tableName)
        {
            string sql = @$"USE MusicBrainz;
                            SELECT
                               * 
                            FROM
                               {tableName};";

            DataTable output = GetQueryResult(sql);

            List<object> records = new List<object>();

            //switch (tableName)
            //{
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //    case "Area":
            //        break;
            //}

            foreach (DataRow row in output.Rows)
            {
                row.ToObject<Area>();
            }
            return new List<object>();

        }

        public T GetRecordById<T>(int id) where T : new()
        {
            Type entityType = typeof(T);

            string entityTypeName = entityType.Name;

            string sql = @$"USE MusicBrainz;
                            SELECT
                               * 
                            FROM
                               {entityTypeName}
                            WHERE Id = {id};";


            T? result = GetQueryResult(sql).Rows [0].ToObject<T>();

            return result;
        }


        public IEnumerable<T> GetTableRecords<T>() where T : new()
        {
            DataTable originalOutput,
                        mappedOutput;

            Type entityType = typeof(T);

            List<T> entitiesList = new(); // this will be returned

            string entityTypeName = entityType.Name,

                    sql = @$"USE MusicBrainz;
                            SELECT
                               * 
                            FROM
                               {entityTypeName};";



            originalOutput = GetQueryResult(sql);

            // a structure copy of original table
            mappedOutput = originalOutput.Clone();

            //deleting gid
            mappedOutput.Columns.Remove("gid");

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
                entitiesList.Add(mappedRow.ToObject<T>());
            }



            return entitiesList;
        }

    }

}
