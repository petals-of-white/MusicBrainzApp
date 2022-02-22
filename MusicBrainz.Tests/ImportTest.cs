using System.Collections.Generic;
using MusicBrainz.BLL.DbEntitySerialization;
using MusicBrainz.Common.Entities;
using MusicBrainz.Common.Enums;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class ImportTest
    {
        private readonly ITestOutputHelper output;
        public ImportTest(ITestOutputHelper output)
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

        [Fact]
        //[MemberData(nameof(GetTablesEnum))]
        public void ImportEntities_ShouldWork()
        {
            DbExportImportConfig config = new();

            config.AddEntitiesToImport(Tables.Area
                , new List<TableEntity>()
                {
                    new Area {Name="MegaKekos", SortName="MegaKekos", Comment="Wawawa", Ended=false}
                });

            config.AddEntitiesToImport(Tables.Artist
                , new List<TableEntity>()
            {
                    new Artist {Name="MegaMeme", SortName="MegaMeme", Comment="Rarara"

                    } });

            //config.AddEntitiesToImport(Tables.Label, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.Place, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.Recording, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.Release, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.ReleaseGroup, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.Url, new List<TableEntity>());

            //config.AddEntitiesToImport(Tables.Work, new List<TableEntity>());


            DbEntitiesSerializer main = new(config);

            main.ImportSerializedTableEntities();
        }
    }
}
