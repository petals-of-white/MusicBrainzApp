using System.Collections.Generic;
using System.Linq;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.DAL;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class DbTest
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

        public DbTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TablesInfo_ShouldWork()
        {
            DbAccess db = new();
            var res = db.GetTablesInfo();
            Assert.True(true);
        }

        [Fact]
        public void GetTablesInfo_ReturnsCompleteInfo()
        {
            DbAccess db = new();

            var tableInfo = db.GetTablesInfo();

            bool propertiesDontContainNulls = tableInfo.Select(x => new object [] { x.Name, x.NumberOfRecords }).All(properties => properties.Contains(null) == false);

            Assert.NotEmpty(tableInfo);

            Assert.True(propertiesDontContainNulls);

            foreach (var table in tableInfo)
            {
                output.WriteLine($"{table.Name}, {table.NumberOfRecords}");
            }
        }

        [Fact]
        public void GetTableRecordsWithSimpleEntities_ShouldWork()
        {
            DbAccess db = new();

            var area = db.GetTableRecords<Area>();
            var works = db.GetTableRecords<Work>();
            var urls = db.GetTableRecords<Url>();
            var releaseGroups = db.GetTableRecords<ReleaseGroup>();
            var recordings = db.GetTableRecords<Recording>();

            Assert.NotEmpty(area);
            Assert.NotEmpty(works);
            Assert.NotEmpty(urls);
            Assert.NotEmpty(releaseGroups);
            Assert.NotEmpty(recordings);
        }

        [Fact]
        public void GetTableRecordsWithComplexEntities_ShouldWork()
        {
            DbAccess db = new();

            var artists = db.GetTableRecords<Artist>();
            var releases = db.GetTableRecords<Release>();
            var labels = db.GetTableRecords<Label>();
            var places = db.GetTableRecords<Place>();

            Assert.NotEmpty(artists);
            Assert.NotEmpty(releases);
            Assert.NotEmpty(labels);
            Assert.NotEmpty(places);
        }

        [Theory]
        [InlineData(Tables.Area)]
        [InlineData(Tables.Artist)]
        [InlineData(Tables.Label)]
        [InlineData(Tables.Place)]
        [InlineData(Tables.Release)]
        [InlineData(Tables.ReleaseGroup)]
        [InlineData(Tables.Recording)]
        [InlineData(Tables.Work)]
        [InlineData(Tables.Url)]
        public void SimpleReaderToEntityConversion_ShouldWork(Tables tableOption)
        {
            DbAccess db = new();

            var entities = db.GetTableRecords(tableOption, 20, 3);

            Assert.NotEmpty(entities);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetRecordById_NotNull(Tables tableOption)
        {
            DbAccess db = new();
            var result = db.GetRecordById(20, tableOption);
            Assert.True(true);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetEntitiesList_ShouldWorkWithForeignKeys(Tables tableOption)
        {
            DbAccess db = new();
            var result = db.GetTableRecords(tableOption, 1000, 3);
            Assert.DoesNotContain(null, result);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetRecordsList_ShouldBePerfectAndNotEmpty(Tables table)
        {
            DbAccess db = new();
            int numberOfRecords = 50000;
            int [] ids = new int [numberOfRecords];
            for (int i = 0; i < numberOfRecords; i++)
            {
                ids [i] = i + 1;
            }
            var entities = db.GetRecordsList(ids, table);
            Assert.NotEmpty(entities);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetMultipleRecordsById_ShowShouldNotBeEmpty(Tables table)
        {
            DbAccess db = new();

            int numberOfRecords = 50000;

            object? [] entities = new object? [numberOfRecords];

            for (int i = 0; i < numberOfRecords; i++)
            {
                entities [i] = db.GetRecordById(i + 1, table);
            }
            Assert.NotEmpty(entities);
        }

    }
}
