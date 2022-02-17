using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MusicBrainz.BLL.DbEntitySerialization;
using MusicBrainz.Common.Enums;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class ExportTest
    {
        private readonly ITestOutputHelper output;

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

        public ExportTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void DbEntitiesSerializer_OneTable_SerializationWork(Tables table)
        {
            Stopwatch stopwatch = new();
            Random rand = new();
            var recordsPerPage = rand.Next(1, 1000);
            var pageNumber = rand.Next(1, 30);

            DbExportImportConfig config = new();
            config.AddTableToExport(new Tables [] { table });
            config.EnablePaging(recordsPerPage, pageNumber);

            stopwatch.Start();

            DbEntitiesSerializer dbSerializer = new(config);

            stopwatch.Stop();
            output.WriteLine(stopwatch.Elapsed.ToString());

            dbSerializer.SerializeTableEntities();

            string json = File.ReadAllText($"export/{table}.json");

            bool fileEmpty = string.IsNullOrWhiteSpace(json) || json.Length < 5;
            Assert.False(fileEmpty);
        }
    }
}
