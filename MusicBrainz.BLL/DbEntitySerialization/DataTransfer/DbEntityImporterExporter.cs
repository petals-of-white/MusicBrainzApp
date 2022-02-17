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
        public IEnumerable<object> Export(Tables table, int? recordsPerPage, int? pageNumber) =>
            _db.GetTableRecords(table, recordsPerPage, pageNumber);

        public void Import(Tables table, IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ITableInfo> GetTablesInfo()
        {
            return _db.GetTablesInfo();
        }
    }
}