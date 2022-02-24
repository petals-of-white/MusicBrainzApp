using System.Data;
using Microsoft.Data.SqlClient;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.DAL
{
    /// <summary>
    /// Contains logic that helps with converting SqlReader to corresponding entity type.
    /// </summary>
    internal static class DataReaderToEntityConverterWithForeignKeysExport
    {
        public delegate (T, IEnumerable<int?>) ReaderToEntityConverter<T>(IDataReader reader);

        public delegate IEnumerable<int?> ForeignKeyExporter(IDataReader reader);

        public static IEnumerable<(T entity, IEnumerable<int?> foreignKeys)> Select<T>(this IDataReader reader, ReaderToEntityConverter<T> projection) where T : new()
        {
            //List<IEnumerable<int?>> output = new List<IEnumerable<int>>();

            while (reader.Read())
            {
                var (entity, list) = projection(reader);
                yield return (entity, list);
            }
        }

        private static void test()
        {
            using var con = new SqlConnection();
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "askldfjasdlk;fj";
            var reader = cmd.ExecuteReader();

            IEnumerable<int?> foreignKeys;

            var res = reader.Select(AreaFromReader);

            var fkTable = res.Select(x => x.foreignKeys);
        }

        /// <summary>
        /// Converts reader row to an entity T
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        ///
        private static T? ConvertValue<T>(object raw)
            =>
            (raw != DBNull.Value) ? (T) raw : default(T);

        public static (Area, IEnumerable<int?>) AreaFromReader(IDataReader reader)
        {
            IEnumerable<int?> fkList = new List<int?>();

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

            return (area, fkList);
        }

        public static (Artist, IEnumerable<int?>) ArtistFromReader(IDataReader reader)
        {
            Artist artist = new();
            List<int?> fkList = new List<int?>();

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

            //!!!!!!!!!!!!!!!!!!!!

            artist.Ended = (bool) reader ["Ended"];
            artist.EditsPending = (int) reader ["EditsPending"];
            artist.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            fkList.AddRange(new int? [] { areaFk, beginAreaFk, endAreaFk });

            return (artist, fkList);
        }

        public static (Label, IEnumerable<int?>) LabelFromReader(IDataReader reader)
        {
            Label label = new();

            List<int?> fkList = new List<int?>();

            label.Id = (int) reader ["Id"];
            label.Name = (string) reader ["Name"];
            label.SortName = (string) reader ["SortName"];
            label.Comment = (string) reader ["Comment"];

            label.LabelCode = ConvertValue<int?>(reader ["LabelCode"]);

            int? areaFk = ConvertValue<int?>(reader ["Area"]);
            // !!!!!!!!!!!!!!
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

            fkList.Add(areaFk);

            return (label, fkList);
        }

        public static (Place, IEnumerable<int?>) PlaceFromReader(IDataReader reader)
        {
            Place place = new();

            List<int?> fkList = new List<int?>();

            place.Id = (int) reader ["Id"];
            place.Name = (string) reader ["Name"];
            place.Comment = (string) reader ["Comment"];

            int? areaFk = ConvertValue<int?>(reader ["Area"]);
            // !!!!!!!!!!!!!!
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

            fkList.Add(areaFk);

            return (place, fkList);
        }

        public static (Recording, IEnumerable<int?>) RecordingFromReader(IDataReader reader)
        {
            Recording recording = new();

            IEnumerable<int?> fkList = new List<int?>();

            recording.Id = (int) reader ["Id"];
            recording.Name = (string) reader ["Name"];
            recording.Comment = (string) reader ["Comment"];
            recording.Length = ConvertValue<int?>(reader ["Length"]);
            recording.Video = (bool) reader ["Video"];
            recording.EditsPending = (int) reader ["EditsPending"];
            recording.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return (recording, fkList);
        }

        public static (Release, IEnumerable<int?>) ReleaseFromReader(IDataReader reader)
        {
            Release release = new();

            List<int?> fkList = new List<int?>();

            release.Id = (int) reader ["Id"];
            release.Name = (string) reader ["Name"];
            release.Comment = (string) reader ["Comment"];

            int releaseGroupFk = ConvertValue<int>(reader ["ReleaseGroup"]);

            // !!!!!!!!!!!!!!!!!
            // !!!!!!!!!!!!!!!!!

            release.Barcode = ConvertValue<string?>(reader ["Barcode"]);

            release.Quality = (short) reader ["Quality"];

            release.EditsPending = (int) reader ["EditsPending"];
            release.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            fkList.Add(releaseGroupFk);

            return (release, fkList);
        }

        public static (ReleaseGroup, IEnumerable<int?>) ReleaseGroupFromReader(IDataReader reader)
        {
            ReleaseGroup releaseGroup = new();

            IEnumerable<int?> fkList = new List<int?>();

            releaseGroup.Id = (int) reader ["Id"];
            releaseGroup.Name = (string) reader ["Name"];
            releaseGroup.Comment = (string) reader ["Comment"];
            releaseGroup.EditsPending = (int) reader ["EditsPending"];
            releaseGroup.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return (releaseGroup, fkList);
        }

        public static (Url, IEnumerable<int?>) UrlFromReader(IDataReader reader)
        {
            Url url = new();

            IEnumerable<int?> fkList = new List<int?>();

            url.Id = (int) reader ["Id"];
            url.UrlValue = (string) reader ["UrlValue"];
            url.EditsPending = (int) reader ["EditsPending"];
            url.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return (url, fkList);
        }

        public static (Work, IEnumerable<int?>) WorkFromReader(IDataReader reader)
        {
            Work work = new();

            IEnumerable<int?> fkList = new List<int?>();

            work.Id = (int) reader ["Id"];
            work.Name = (string) reader ["Name"];
            work.Comment = (string) reader ["Comment"];
            work.EditsPending = (int) reader ["EditsPending"];
            work.LastUpdated = ConvertValue<DateTime?>(reader ["LastUpdated"]);

            return (work, fkList);
        }

        public static ITableInfo TableInfoFromReader(IDataReader reader)
        {
            DbTableInfo tableInfo = new()
            {
                Name = (Tables) Enum.Parse(typeof(Tables), (string) reader ["Name"]),
                NumberOfRecords = null
            };

            return tableInfo;
        }
    }
}