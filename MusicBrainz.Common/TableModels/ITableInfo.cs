using MusicBrainz.Common.Enums;

namespace MusicBrainz.Common.TableModels
{
    public interface ITableInfo
    {
        Tables Name { get; }
        int? NumberOfRecords { get; }
    }
}
