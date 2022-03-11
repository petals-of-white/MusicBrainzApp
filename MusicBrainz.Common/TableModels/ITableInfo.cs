using MusicBrainz.Common.Enums;

namespace MusicBrainz.Common.TableModels
{
    /// <summary>
    /// Represents a db table name and number of records in it.
    /// </summary>
    public interface ITableInfo
    {
        Tables Name { get; }
        int? NumberOfRecords { get; }
    }
}