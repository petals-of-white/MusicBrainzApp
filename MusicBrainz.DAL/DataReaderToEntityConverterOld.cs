using System.Data;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.DAL
{
    /// <summary>
    /// Contains logic that helps with converting SqlReader to corresponding entity type.
    /// </summary>
    internal static class DataReaderToEntityConverterOld
    {
        public delegate object? ForeignKeySubstitutioner(int id, Tables sourceTable);

        public delegate T ReaderToEntityConverter<T>(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner);

        public delegate void ForeignKeyExporter<T>(IDataReader reader);

        public static IEnumerable<T> Select<T>(this IDataReader reader, ReaderToEntityConverter<T> projection, ForeignKeyExporter<T> foreignKeyExporter, IEnumerable<int> outputCollection) where T : new()
        {
            while (reader.Read())
            {
                var result = projection(reader, null);

                foreignKeyExporter(reader);

                yield return result;
            }
        }

        /// <summary>
        /// This extension method allows IDataReader to be queried like any other IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="projection"></param>
        /// <param name="foreignKeySubstitutioner"></param>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(this IDataReader reader, ReaderToEntityConverter<T> projection, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            while (reader.Read())
            {
                var result = projection(reader, foreignKeySubstitutioner);

                yield return result;
            }
        }

        /// <summary>
        /// Converts reader row to an entity T
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static T? ConvertValue<T>(object raw)
            =>
       (raw != DBNull.Value) ? (T) raw : default(T);

        public static Area AreaFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Area area = new();

            area.Id = (int) reader ["Id"];
            area.Name = (string) reader ["Name"];
            area.SortName = (string) reader ["SortName"];

            area.Comment = (string) reader ["Comment"];

            area.BeginDateYear = ConvertValue<short?>(reader ["BeginDateYear"]);
            area.BeginDateMonth = ConvertValue<short?>(reader ["BeginDateMonth"]);
            area.BeginDateDay = ConvertValue<short?>(reader ["BeginDateDay"]);

            area.EndDateYear = ConvertValue<short?>(reader ["EndDateYear"]);
            area.EndDateMonth = ConvertValue<short?>(reader ["EndDateMonth"]);
            area.EndDateDay = ConvertValue<short?>(reader ["EndDateDay"]);

            area.Ended = (bool) reader ["Ended"];
            area.EditsPending = (int) reader ["EditsPending"];
            area.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return area;
        }

        public static Artist ArtistFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Artist artist = new();

            artist.Id = (int) reader ["Id"];
            artist.Name = (string) reader ["Name"];
            artist.SortName = (string) reader ["SortName"];

            artist.Comment = (string) reader ["Comment"];

            artist.BeginDateYear = ConvertValue<short?>(reader ["BeginDateYear"]);
            artist.BeginDateMonth = ConvertValue<short?>(reader ["BeginDateMonth"]);
            artist.BeginDateDay = ConvertValue<short?>(reader ["BeginDateDay"]);

            artist.EndDateYear = ConvertValue<short?>(reader ["EndDateYear"]);
            artist.EndDateMonth = ConvertValue<short?>(reader ["EndDateMonth"]);
            artist.EndDateDay = ConvertValue<short?>(reader ["EndDateDay"]);

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!

            int? areaFk = ConvertValue<int?>(reader ["Area"]);
            int? beginAreaFk = ConvertValue<int?>(reader ["BeginArea"]);
            int? endAreaFk = ConvertValue<int?>(reader ["EndArea"]);

            artist.Area = (areaFk is not null) ? (Area) foreignKeySubstitutioner(areaFk.Value, Tables.Area) : null;

            artist.BeginArea = (beginAreaFk is not null) ? (Area) foreignKeySubstitutioner(beginAreaFk.Value, Tables.Area) : null;
            artist.EndArea = (endAreaFk is not null) ? (Area) foreignKeySubstitutioner(endAreaFk.Value, Tables.Area) : null;
            //!!!!!!!!!!!!!!!!!!!!

            artist.Ended = (bool) reader ["Ended"];
            artist.EditsPending = (int) reader ["EditsPending"];
            artist.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return artist;
        }

        public static Label LabelFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Label label = new();

            label.Id = (int) reader ["Id"];
            label.Name = (string) reader ["Name"];
            label.SortName = (string) reader ["SortName"];
            label.Comment = (string) reader ["Comment"];

            label.LabelCode = ConvertValue<int?>(reader ["LabelCode"]);

            int? areaFk = ConvertValue<int?>(reader ["Area"]);
            // !!!!!!!!!!!!!!
            label.Area = (areaFk is not null) ? (Area) foreignKeySubstitutioner(areaFk.Value, Tables.Area) : null;
            // !!!!!!!!!!!!!!

            label.BeginDateYear = ConvertValue<short?>(reader ["BeginDateYear"]);
            label.BeginDateMonth = ConvertValue<short?>(reader ["BeginDateMonth"]);
            label.BeginDateDay = ConvertValue<short?>(reader ["BeginDateDay"]);

            label.EndDateYear = ConvertValue<short?>(reader ["EndDateYear"]);
            label.EndDateMonth = ConvertValue<short?>(reader ["EndDateMonth"]);
            label.EndDateDay = ConvertValue<short?>(reader ["EndDateDay"]);

            label.Ended = (bool) reader ["Ended"];
            label.EditsPending = (int) reader ["EditsPending"];
            label.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return label;
        }

        public static Place PlaceFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Place place = new();

            place.Id = (int) reader ["Id"];
            place.Name = (string) reader ["Name"];
            place.Comment = (string) reader ["Comment"];

            int? areaFk = ConvertValue<int?>(reader ["Area"]);
            // !!!!!!!!!!!!!!
            place.Area = (areaFk is not null) ? (Area) foreignKeySubstitutioner(areaFk.Value, Tables.Area) : null;
            // !!!!!!!!!!!!!!

            place.Address = (string) reader ["Address"];
            place.Coordinates = ConvertValue<string?>(reader ["Address"]);

            place.BeginDateYear = ConvertValue<short?>(reader ["BeginDateYear"]);
            place.BeginDateMonth = ConvertValue<short?>(reader ["BeginDateMonth"]);
            place.BeginDateDay = ConvertValue<short?>(reader ["BeginDateDay"]);

            place.EndDateYear = ConvertValue<short?>(reader ["EndDateYear"]);
            place.EndDateMonth = ConvertValue<short?>(reader ["EndDateMonth"]);
            place.EndDateDay = ConvertValue<short?>(reader ["EndDateDay"]);

            place.Ended = (bool) reader ["Ended"];
            place.EditsPending = (int) reader ["EditsPending"];
            place.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return place;
        }

        public static Recording RecordingFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Recording recording = new();

            recording.Id = (int) reader ["Id"];
            recording.Name = (string) reader ["Name"];
            recording.Comment = (string) reader ["Comment"];
            recording.Length = ConvertValue<int?>(reader ["Length"]);
            recording.Video = (bool) reader ["Video"];
            recording.EditsPending = (int) reader ["EditsPending"];
            recording.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return recording;
        }

        public static Release ReleaseFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Release release = new();

            release.Id = (int) reader ["Id"];
            release.Name = (string) reader ["Name"];
            release.Comment = (string) reader ["Comment"];

            int releaseGroupFk = ConvertValue<int>(reader ["ReleaseGroup"]);

            // !!!!!!!!!!!!!!!!!
            release.ReleaseGroup = (ReleaseGroup) foreignKeySubstitutioner(releaseGroupFk, Tables.ReleaseGroup);
            // !!!!!!!!!!!!!!!!!

            release.Barcode = ConvertValue<string?>(reader ["Barcode"]);

            release.Quality = (short) reader ["Quality"];

            release.EditsPending = (int) reader ["EditsPending"];
            release.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return release;
        }

        public static ReleaseGroup ReleaseGroupFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            ReleaseGroup releaseGroup = new();

            releaseGroup.Id = (int) reader ["Id"];
            releaseGroup.Name = (string) reader ["Name"];
            releaseGroup.Comment = (string) reader ["Comment"];
            releaseGroup.EditsPending = (int) reader ["EditsPending"];
            releaseGroup.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return releaseGroup;
        }

        public static Url UrlFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Url url = new();

            url.Id = (int) reader ["Id"];
            url.UrlValue = (string) reader ["UrlValue"];
            url.EditsPending = (int) reader ["EditsPending"];
            url.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return url;
        }

        public static Work WorkFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            Work work = new();

            work.Id = (int) reader ["Id"];
            work.Name = (string) reader ["Name"];
            work.Comment = (string) reader ["Comment"];
            work.EditsPending = (int) reader ["EditsPending"];
            work.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return work;
        }

        public static ITableInfo TableInfoFromReader(IDataReader reader, ForeignKeySubstitutioner? foreignKeySubstitutioner = null)
        {
            DbTableInfo tableInfo = new()
            {
                Name = (Tables) Enum.Parse(typeof(Tables), (string) reader ["TableName"]),
                NumberOfRecords = (int) reader ["Records"]
            };

            return tableInfo;
        }
    }
}
