using System;
using System.Collections.Generic;
using System.IO;
using MusicBrainz.BLL.DbEntitySerialization;
using MusicBrainz.Common.Enums;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class ExportTest
    {
        private static int _pageNumberCheck = random.Next(1, 10);
        private static int _recordsPerPageCheck = random.Next(1, 5000);
        private static Random random = new();
        private readonly ITestOutputHelper output;

        public ExportTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static IEnumerable<object []> GetTablesEnum()
        {
            yield return new object [] { Tables.Area };
            yield return new object [] { Tables.Artist };
            yield return new object [] { Tables.Label };
            yield return new object [] { Tables.Place };
            yield return new object [] { Tables.Recording };
            yield return new object [] { Tables.Release };
            yield return new object [] { Tables.ReleaseGroup };
            yield return new object [] { Tables.Work };
            yield return new object [] { Tables.Url };
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void DbEntitiesSerializer_SerializationWork_OneTable_GenericMapping(Tables table)
        {
            // configuring...
            DbExportImportConfig config = new();
            config.AddTableToExport(new Tables [] { table });
            config.EnablePaging(_recordsPerPageCheck, _pageNumberCheck);

            //serializing ...
            DbEntitiesSerializer dbSerializer = new(config);

            dbSerializer.SerializeTabelEntitiesTypeMapped();

            // Reading json from file
            string json = File.ReadAllText($"export/{table}.json");

            bool fileEmpty = string.IsNullOrWhiteSpace(json) || json.Length < 5;

            Assert.False(fileEmpty);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void DbEntitiesSerializer_SerializationWork_OneTableEnum(Tables table)
        {
            //Random rand = new();
            //var recordsPerPage = rand.Next(1, 1000);
            //var pageNumber = rand.Next(1, 30);

            DbExportImportConfig config = new();
            config.AddTableToExport(new Tables [] { table });
            config.EnablePaging(_recordsPerPageCheck, _pageNumberCheck);

            DbEntitiesSerializer dbSerializer = new(config);

            dbSerializer.SerializeTableEntities();

            string json = File.ReadAllText($"export/{table}.json");

            bool fileEmpty = string.IsNullOrWhiteSpace(json) || json.Length < 5;

            Assert.False(fileEmpty);
        }
    }
}