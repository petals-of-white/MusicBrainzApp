using MusicBrainz.Common.Enums;

namespace MusicBrainz.BLL.DbEntitySerialization
{
    public class DbExportConfig
    {
        public int? PageNumber
        {
            get; private set;
        }

        public bool PagingEnabled
        {
            get => RecordsPerPage is null || PageNumber is null ? false : true;
        }

        public int? RecordsPerPage
        {
            get;
            private set;
        }

        public HashSet<Tables> TablesToExport { get; private set; } = new();

        /// <summary>
        /// adds tables to the hashset, making sure they are unique
        /// </summary>
        /// <param name="tables"></param>
        public void AddTableToExport(Tables [] tables)
        {
            TablesToExport.UnionWith(tables);
        }

        public void AddTableToExport(Tables tables)
        {
            TablesToExport.Add(tables);
        }

        public void EnablePaging(int recordsPerPage, int pageNumber)
        {
            RecordsPerPage = recordsPerPage;
            PageNumber = pageNumber;
        }
    }
}