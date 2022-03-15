using MusicBrainz.Common.Enums;

namespace MusicBrainz.Common.TableModels
{
    public class DbTableInfo : ITableInfo
    {
        public Tables Name { get; init; }

        public int? NumberOfRecords { get; init; }
    }
}