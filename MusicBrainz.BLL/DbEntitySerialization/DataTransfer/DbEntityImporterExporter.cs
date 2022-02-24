using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;
using MusicBrainz.DAL;

namespace MusicBrainz.BLL.DbEntitySerialization.DataTransfer
{
    internal class DbEntityImporterExporter : IDbEntityImporterExporter, IDbTablesInfoGetter
    {
        private DbAccess _db = new();

        /// <summary>
        ///
        /// </summary>
        /// <param name="table"></param>
        /// <param name="recordsPerPage"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws this exception when records per page or page number is less then 1</exception>
        public ICollection<object> Export(Tables table, int? recordsPerPage, int? pageNumber) =>
            _db.GetTableRecords(table, recordsPerPage, pageNumber);

        public ICollection<T> Export<T>(int? recordsPerPage, int? pageNumber) where T : TableEntity =>
                        _db.GetTableRecordsGenericMapProperties<T>(recordsPerPage, pageNumber);

        public IList<ITableInfo> GetTablesInfo()
        {
            return _db.GetDbTablesInfo();
        }

        public void Import(Tables table, ICollection<object> entities)
        {
            _db.InsertEntities(table, entities);
            throw new NotImplementedException();
        }

        public void Import<T>(ICollection<T> entities) where T : TableEntity
        {
            _db.InsertEntities<T>(entities);
        }
    }
}