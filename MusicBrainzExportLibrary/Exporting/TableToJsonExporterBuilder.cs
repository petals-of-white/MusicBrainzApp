using MusicBrainzDataAcessLibrary;
using MusicBrainzModelsLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public class TableToJsonExporterBuilder : IPaginatedTableExporterBuilder
    {
        private TableToJsonExporter _jsonExporter = new();

        /// <summary>
        /// Sets a path to a json file for the exporter
        /// </summary>
        /// <param name="jsonPath"></param>
        public void JsonPath(string jsonPath)
        {

        }
        /// <summary>
        /// Builds the json exporter
        /// </summary>
        /// <returns></returns>
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
        public void EnablePagination(int recordsPerPage, int pageNumber)
        {
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
            foreach (ITable table in _jsonExporter.TablesInfo)
            {
                UseTable(table.Name);
            }
        }

        public void UseTable(ITable table)
        {
            throw new NotImplementedException();
            //WARNING!!!!!
            // Not sure if this works for STRUCTs

            //if (_jsonExporter.TablesInfo.Values.Contains(table) == false)
            //    _jsonExporter.SelectedTables.Add(table);
        }

        /// <summary>
        /// Add a table with a specific name to export list
        /// </summary>
        /// <param name="tableName"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UseTable(string tableName)
        {
            // something
            var selectedTable = _jsonExporter.TablesInfo.Where(x => x.Name == tableName).FirstOrDefault<ITable>(defaultValue: null);
            if (selectedTable is not null)
            {
                _jsonExporter.SelectedTables.Add(selectedTable);
            }
            else
            {
                throw new ArgumentException("A table with this name doesn't exist", "tableName");
            }

        }

        public void UseTable(AvailableTables table)
        {
            throw new NotImplementedException();
            //_jsonExporter.SelectedTables.Add(); 
        }

        /// <summary>
        /// Return all the information about the table from exporter
        /// </summary>
        /// <returns>A dictionary with int as a key and ITable object as a value</returns>
        public IList<ITable> GetTableInfo()
        {
            return _jsonExporter.TablesInfo;

        }

    }
}
