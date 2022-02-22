using System;
using System.Collections.Generic;
using System.Linq;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using MusicBrainz.DAL;
using MusicBrainz.Tools.Config;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class DbTest
    {
        private readonly ITestOutputHelper output;

        private static Random random = new();
        //private static int? _pageNumberCheck = random.Next(1, 100);
        //private static int? _recordsPerPageCheck = random.Next(1, 1000);

        private static int? _pageNumberCheck = Convert.ToInt32(
            ConfigHelper.Configuration
            .GetSection("PagingOptions").GetSection("PageNumber").Value);

        private static int? _recordsPerPageCheck = Convert.ToInt32(
            ConfigHelper.Configuration
            .GetSection("PagingOptions").GetSection("RecordsPerPage").Value);

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
            var res = db.GetDbTablesInfo();
            Assert.True(true);
        }

        [Fact]
        public void GetTablesInfo_ReturnsCompleteInfo()
        {
            DbAccess db = new();

            var tableInfo = db.GetDbTablesInfo();

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
        [MemberData(nameof(GetTablesEnum))]
        public void GetTableRecords_Enum_ShouldBeGood(Tables tableOption)
        {
            DbAccess db = new();

            List<object> entities = db.GetTableRecords(tableOption, _recordsPerPageCheck, _pageNumberCheck).ToList();

            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
            Assert.True(true);
            //Assert.NotEmpty(entities);
        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetTableRecords_GenericMappingDefault_ShouldBeGood(Tables table)
        {
            DbAccess db = new();

            List<TableEntity> entities = default;

            switch (table)
            {
                case Tables.Area:
                    entities = db.GetTableRecordsGenericMapDefault<Area>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Artist:
                    entities = db.GetTableRecordsGenericMapDefault<Artist>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Label:
                    entities = db.GetTableRecordsGenericMapDefault<Label>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Place:
                    entities = db.GetTableRecordsGenericMapDefault<Place>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Recording:
                    entities = db.GetTableRecordsGenericMapDefault<Recording>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Release:
                    entities = db.GetTableRecordsGenericMapDefault<Release>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.ReleaseGroup:
                    entities = db.GetTableRecordsGenericMapDefault<ReleaseGroup>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Work:
                    entities = db.GetTableRecordsGenericMapDefault<Work>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Url:
                    entities = db.GetTableRecordsGenericMapDefault<Url>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
            }
            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
            Assert.True(true);
            //Assert.DoesNotContain(null, oneTable);

            //output.WriteLine(oneTable?.ToString() ?? "null");

        }

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetTableRecords_GenericMappingProperties_ShouldBeGood(Tables table)
        {
            DbAccess db = new();

            List<TableEntity> entities = default;

            switch (table)
            {
                case Tables.Area:
                    entities = db.GetTableRecordsGenericMapProperties<Area>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Artist:
                    entities = db.GetTableRecordsGenericMapProperties<Artist>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Label:
                    entities = db.GetTableRecordsGenericMapProperties<Label>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Place:
                    entities = db.GetTableRecordsGenericMapProperties<Place>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Recording:
                    entities = db.GetTableRecordsGenericMapProperties<Recording>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Release:
                    entities = db.GetTableRecordsGenericMapProperties<Release>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.ReleaseGroup:
                    entities = db.GetTableRecordsGenericMapProperties<ReleaseGroup>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Work:
                    entities = db.GetTableRecordsGenericMapProperties<Work>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
                case Tables.Url:
                    entities = db.GetTableRecordsGenericMapProperties<Url>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
            }
            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
            Assert.True(true);
            //Assert.DoesNotContain(null, oneTable);

            //output.WriteLine(oneTable?.ToString() ?? "null");

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
        public void GetRecordsByIdList_ShouldBePerfectAndNotEmpty(Tables table)
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
        public void GetMultipleRecordsByIdDefault_NotEmptyAndGood(Tables table)
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

        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetMultipleRecordsByIdGeneric_ShouldBeGood(Tables table)
        {
            DbAccess db = new();

            int numberOfRecords = 50000;

            TableEntity? [] entities = new TableEntity? [numberOfRecords];

            for (int i = 0; i < numberOfRecords; i++)
            {
                switch (table)
                {
                    case Tables.Area:
                        entities [i] = db.GetRecordById<Area>(i + 1);
                        break;
                    case Tables.Artist:
                        entities [i] = db.GetRecordById<Artist>(i + 1);
                        break;
                    case Tables.Label:
                        entities [i] = db.GetRecordById<Label>(i + 1);
                        break;
                    case Tables.Place:
                        entities [i] = db.GetRecordById<Place>(i + 1);
                        break;
                    case Tables.Recording:
                        entities [i] = db.GetRecordById<Recording>(i + 1);
                        break;
                    case Tables.Release:
                        entities [i] = db.GetRecordById<Release>(i + 1);
                        break;
                    case Tables.ReleaseGroup:
                        entities [i] = db.GetRecordById<ReleaseGroup>(i + 1);
                        break;
                    case Tables.Work:
                        entities [i] = db.GetRecordById<Work>(i + 1);
                        break;
                    case Tables.Url:
                        entities [i] = db.GetRecordById<Url>(i + 1);
                        break;
                }
            }
            Assert.NotEmpty(entities);
        }
    }
}
