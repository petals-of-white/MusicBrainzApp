using MusicBrainz.Common.Entities;
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

        //Make sure this is inaccessible for edit from outer classe
        public Dictionary<Tables, ICollection<TableEntity>> EntitiesToImport { get; private set; } =
            new Dictionary<Tables, ICollection<TableEntity>>();

        public Dictionary<Tables, string> SerializedEntitiesToImport { get; private set; } =
            new Dictionary<Tables, string>();

        public void EnablePaging(int recordsPerPage, int pageNumber)
        {
            RecordsPerPage = recordsPerPage;
            PageNumber = pageNumber;
        }

        /// <summary>
        /// adds tables to the hashset, making sure they are unique
        /// </summary>
        /// <param name="tables"></param>
        public void AddTableToExport(Tables [] tables)
        {
            TablesToExport.UnionWith(tables);
        }

        public void AddEntitiesToImport(Tables table, ICollection<TableEntity> entities)
        {
            EntitiesToImport.Add(table, entities);
        }

        public void AddEntitiesToImport(Tables table, string jsonObject)
        {
            SerializedEntitiesToImport.Add(table, jsonObject);
        }
    }
}