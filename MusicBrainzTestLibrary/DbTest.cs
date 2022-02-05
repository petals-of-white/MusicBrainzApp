using MusicBrainzDataAcessLibrary;
using MusicBrainzDataAcessLibrary.Entities;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainzTestLibrary
{

    public class DbTest
    {
        private readonly ITestOutputHelper output;

        public DbTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TablesInfo_ShouldWork()
        {
            DBAccess db = new();
            var res = db.GetTablesInfo();
            Assert.True(true);
        }

        [Fact]
        public void GetTableRecordsWithSimpleEntities_ShouldWork()
        {
            DBAccess db = new();

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
            DBAccess db = new();

            var artists = db.GetTableRecords<Artist>();
            var releases = db.GetTableRecords<Release>();
            var labels = db.GetTableRecords<Label>();
            var places = db.GetTableRecords<Place>();

            Assert.NotEmpty(artists);
            Assert.NotEmpty(releases);
            Assert.NotEmpty(labels);
            Assert.NotEmpty(places);


        }



    }
}
