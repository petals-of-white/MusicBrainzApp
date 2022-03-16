using System;
using System.Collections.Generic;
using System.Data;
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
        private static int? _pageNumberCheck = Convert.ToInt32(
            ConfigHelper.Configuration
            .GetSection("PagingOptions").GetSection("PageNumber").Value);

        //private static int? _pageNumberCheck = random.Next(1, 100);
        //private static int? _recordsPerPageCheck = random.Next(1, 1000);
        private static int? _recordsPerPageCheck = Convert.ToInt32(
            ConfigHelper.Configuration
            .GetSection("PagingOptions").GetSection("RecordsPerPage").Value);

        private static Random random = new();
        private readonly ITestOutputHelper output;

        public DbTest(ITestOutputHelper output)
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
        public void GetRecordsByIdList_NotEmpty(Tables table)
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
        public void GetTableRecords_Enum_ShouldBeGood(Tables tableOption)
        {
            DbAccess db = new();

            List<object> entities = db.GetTableRecordsNonGeneric(tableOption, _recordsPerPageCheck, _pageNumberCheck).ToList();

            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
            Assert.True(true);
            //Assert.NotEmpty(entities);
        }

        //[Fact]
        //public void GetTableRecordsWithComplexEntities_ShouldWork()
        //{
        //    DbAccess db = new();
        //}
        [Theory]
        [MemberData(nameof(GetTablesEnum))]
        public void GetTableRecords_GenericMappingDefault_ShouldBeGood(Tables table)
        {
            DbAccess db = new();

            List<TableEntity> entities = default;

            switch (table)
            {
                case Tables.Area:
                    entities = db.GetTableRecordsNoPropMapping<Area>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Artist:
                    entities = db.GetTableRecordsNoPropMapping<Artist>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Label:
                    entities = db.GetTableRecordsNoPropMapping<Label>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Place:
                    entities = db.GetTableRecordsNoPropMapping<Place>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Recording:
                    entities = db.GetTableRecordsNoPropMapping<Recording>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Release:
                    entities = db.GetTableRecordsNoPropMapping<Release>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.ReleaseGroup:
                    entities = db.GetTableRecordsNoPropMapping<ReleaseGroup>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Work:
                    entities = db.GetTableRecordsNoPropMapping<Work>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Url:
                    entities = db.GetTableRecordsNoPropMapping<Url>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
            }
            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
            Assert.True(true);
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
                    entities = db.GetTableRecords<Area>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Artist:
                    entities = db.GetTableRecords<Artist>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Label:
                    entities = db.GetTableRecords<Label>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Place:
                    entities = db.GetTableRecords<Place>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Recording:
                    entities = db.GetTableRecords<Recording>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Release:
                    entities = db.GetTableRecords<Release>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.ReleaseGroup:
                    entities = db.GetTableRecords<ReleaseGroup>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Work:
                    entities = db.GetTableRecords<Work>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;

                case Tables.Url:
                    entities = db.GetTableRecords<Url>(_recordsPerPageCheck, _pageNumberCheck).Cast<TableEntity>().ToList();
                    break;
            }

            output.WriteLine($"Records exported - {entities!.Count} .Records per page - {_recordsPerPageCheck}. Page NUmber - {_pageNumberCheck}");
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

        [Theory]
        [InlineData(Report.PlacesInArea)]
        [InlineData(Report.NumberOfArtistsWithAreaEnded)]
        [InlineData(Report.ReleaseGroups_ReleasesAvgEditsPending)]
        public void Report_ReturnsSomething(Report report)
        {
            var db = new DbAccess();
            DataTable reportOutput = new();

            switch (report)
            {
                case Report.PlacesInArea:
                    reportOutput = db.GetPlacesInArea();
                    break;

                case Report.NumberOfArtistsWithAreaEnded:
                    reportOutput = db.GetNumberOfArtistsWithAreaEnded();
                    break;

                case Report.ReleaseGroups_ReleasesAvgEditsPending:
                    reportOutput = db.GetReleaseGroups_ReleasesAvgEditsPending();
                    break;
            }
            output.WriteLine(reportOutput.ToString());
            Assert.True(reportOutput.Rows.Count > 0);
        }

        [Fact]
        public void TablesInfo_ShouldWork()
        {
            DbAccess db = new();
            var res = db.GetDbTablesInfo();
            Assert.True(true);
        }
    }
}
