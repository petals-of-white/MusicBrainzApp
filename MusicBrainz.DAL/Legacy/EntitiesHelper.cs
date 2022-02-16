using System.Data;
using MusicBrainz.Tools;
using MusicBrainz.Tools.Logging;

namespace MusicBrainz.DAL.Legacy
{
    public class EntitiesHelper
    {
        protected LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        public object TransformDataTableToEntities<T>(DataTable dataTable)
        {
            var newTable = dataTable.Clone();

            try
            {
                //deleting gid
                newTable.Columns.Remove("gid");
            }

            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
            }

            foreach (var entity in dataTable.Rows)
            {

            }

            //TransformColumnTypes<T>(newTable);

            throw new NotImplementedException();
        }

        public IList<T> DataTableToEntities<T>(DataTable dataTable) where T : new()
        {
            var entities = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                entities.Add(row.ToObject<T>());
            }
            return entities;
        }

        /// <summary>
        /// Changes columns types in DataTable according to entity type, that is T
        /// In other words, the methods replaces foreign key type (int) values with an actual entity type (T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mappedOutput"></param>
        private DataTable TransformColumnTypes<T>(DataTable rawTable, out IEnumerable<DataColumn> affectedColumns)
        {
            Type entityType = typeof(T);
            DataTable transformedTable = rawTable.Clone();

            List<DataColumn> affected = new();

            foreach (var entityProperty in entityType.GetProperties())
            {
                if (TablesEntitiesInfo.AllTypes.Contains(entityProperty.PropertyType))
                {
                    DataColumn? affectedColumn = transformedTable.Columns [entityProperty.Name];
                    if (affectedColumn is not null)
                    {
                        affectedColumn.DataType = entityProperty.PropertyType;

                        // adding to affected columns list
                        affected.Add(affectedColumn);
                    }
                }
            }
            affectedColumns = affected;
            return transformedTable;

        }
    }
}