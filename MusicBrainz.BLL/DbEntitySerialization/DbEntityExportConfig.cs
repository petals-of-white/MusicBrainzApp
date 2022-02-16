using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization
{
    public class DbExportImportConfig
    {
        public HashSet<Tables> TablesToExport { get; private set; } = new();

        public bool PagingEnabled
        {
            get => RecordsPerPage is null || PageNumber is null ? false : true;
        }

        public int? RecordsPerPage
        {
            get;
            private set;
        }

        public int? PageNumber
        {
            get; private set;
        }

        public void EnablePaging(int recordsPerPage, int pageNumber)
        {
            RecordsPerPage = recordsPerPage;
            PageNumber = pageNumber;
        }

        public void AddTableToExport(Tables [] tables)
        {
            // adds tables to the hashset, making sure they are unique
            TablesToExport.UnionWith(tables);
        }
    }
}