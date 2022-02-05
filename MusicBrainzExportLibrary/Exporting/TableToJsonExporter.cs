using MusicBrainzDataAcessLibrary;
using MusicBrainzExportLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public class TableToJsonExporter : ITableExporter
    {
        //private bool _paginationEnabled;

        //private int? _recordsPerPage;

        //private int? _pageNumber;
        //public string [] SelectedTables { get; internal set; }

        private DBAccess _db = new();

        public int? PageNumber { get; internal set; }

        public bool PaginationEnabled { get; internal set; } = false;

        public int? RecordsPerPage { get; internal set; }

        public IDictionary<int, ITable> TablesInfo { get; internal set; } = new Dictionary<int, ITable>();

        public IList<ITable> SelectedTables { get; internal set; } = new List<ITable>();

        public void Export()
        {
            foreach (ITable table in SelectedTables)
            {
                if (PaginationEnabled == true)
                {

                }
                else
                {

                }
            }
            //return new Table();
        }

    }

}
