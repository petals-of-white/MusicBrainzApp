using MusicBrainzExportLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public class TableToJsonExporterBuilder : ITableExporterBuilder
    {
        private TableToJsonExporter _jsonExporter = new();
        public ITableExporter Build()
        {
            return _jsonExporter;
        }

        public void DisablePagination()
        {
            _jsonExporter.PaginationEnabled = false;
            _jsonExporter.RecordsPerPage = null;
            _jsonExporter.PageNumber = null;
        }

        public void EnablePagination(int recordsPerPage, int pageNumber)
        {
            _jsonExporter.PaginationEnabled = true;
            _jsonExporter.RecordsPerPage = recordsPerPage;
            _jsonExporter.PageNumber = pageNumber;
        }

        public void UseAllTables()
        {
            foreach (ITable table in _jsonExporter.TablesInfo.Values)
            {
                UseTable(table);
            }
        }

        public void UseTable(ITable table)
        {

            //WARNING!!!!!
            // Not sure if this works for STRUCTs

            if (_jsonExporter.TablesInfo.Values.Contains(table) == false)
                _jsonExporter.SelectedTables.Add(table);
        }

        /// <summary>
        /// Return all the information about the table from exporter
        /// </summary>
        /// <returns>A dictionary with int as a key and ITable object as a value</returns>
        public IDictionary<int, ITable> GetTableInfo()
        {
            return _jsonExporter.TablesInfo;
        }

    }
}
