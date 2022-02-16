using MusicBrainz.BLL.Exceptions;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.BLL.Exporting
{
    public class TableToJsonExporterBuilder : IPaginatedTableExporterBuilder
    {
        private TableToJsonExporter _jsonExporter = new();

        ///// <summary>
        ///// Sets a path to a json file for the exporter
        ///// </summary>
        ///// <param name="jsonPath"></param>
        //public void JsonPath(string jsonPath)
        //{

        //}




        /// <summary>
        /// Builds the json exporter
        /// </summary>
        /// <returns>An instance of TableToJsonExporter.</returns>
        public IPaginatedTableExporter Build()
        {
            return _jsonExporter;
        }
        /// <summary>
        /// Disables pagination
        /// </summary>
        public void DisablePagination()
        {
            _jsonExporter.PaginationEnabled = false;
            _jsonExporter.RecordsPerPage = null;
            _jsonExporter.PageNumber = null;
        }

        /// <summary>
        /// Enables pagination
        /// </summary>
        /// <param name="recordsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <exception cref="UserFriendlyException">This exception is thrown when recordsPerPage or pageNumber is less than 1</exception>
        public void EnablePagination(int recordsPerPage, int pageNumber)
        {
            if (recordsPerPage < 1)
            {
                var innerEx = new ArgumentOutOfRangeException(nameof(recordsPerPage), "Value is out of range.");
                var resultEx = new UserFriendlyException("You can not have zero or less records per page.", innerEx);
                throw resultEx;
            }

            if (pageNumber < 1)
            {
                var innerEx = new ArgumentOutOfRangeException(nameof(pageNumber), "Value is out of range.");
                var resultEx = new UserFriendlyException("You can not select a page with number zero or less.", innerEx);
                throw resultEx;
            }

            _jsonExporter.PaginationEnabled = true;
            _jsonExporter.RecordsPerPage = recordsPerPage;
            _jsonExporter.PageNumber = pageNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void UseAllTables()
        {
            foreach (ITableInfo table in _jsonExporter.TablesInfo)
            {
                UseTable(table.Name);
            }
        }

        public void UseTable(ITableInfo table)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a table with a specific name to export list
        /// </summary>
        /// <param name="tableName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UseTable(string tableName)
        {
            // something
            var selectedTable = _jsonExporter.TablesInfo.Where(x => true).FirstOrDefault<DbTableInfo>(defaultValue: null);
            //var selectedTable = _jsonExporter.TablesInfo.Where(x => x.Name == tableName).FirstOrDefault<DbTableInfo>(defaultValue: null);
            if (selectedTable is not null)
            {
                _jsonExporter.SelectedTables.Add(selectedTable);
            }
            else
            {
                throw new ArgumentException("A table with this name doesn't exist", "tableName");
            }
        }

        public void UseTable(Tables table)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return all the information about the table from exporter
        /// </summary>
        /// <returns>A dictionary with int as a key and ITable object as a value</returns>
        public IList<DbTableInfo> GetTableInfo()
        {
            return _jsonExporter.TablesInfo;

        }

    }
}
