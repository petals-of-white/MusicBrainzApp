using System.IO;
using MusicBrainz.BLL.Exporting;
using Xunit;
using Xunit.Abstractions;

namespace MusicBrainz.Tests
{
    public class JSONExportTest
    {
        private readonly ITestOutputHelper output;

        public JSONExportTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("Area")]
        [InlineData("Artist")]
        [InlineData("Label")]
        [InlineData("Place")]
        [InlineData("Recording")]
        [InlineData("Release")]
        [InlineData("ReleaseGroup")]
        [InlineData("Url")]
        [InlineData("Work")]
        public void ExportOneTableToJson_ShouldNotBeEmpty(string tableName)
        {
            TableToJsonExporterBuilder builder = new();

            builder.UseTable(tableName);

            var exporter = (TableToJsonExporter) builder.Build();

            exporter.Export();

            bool empty;

            empty = string.IsNullOrWhiteSpace(File.ReadAllText(tableName + ".json"));

            Assert.False(empty);
        }

        [Fact]
        public void ExportAllTablesToJson_ShouldNotBeEmpty()
        {
            TableToJsonExporterBuilder builder = new();

            builder.UseAllTables();

            var exporter = (TableToJsonExporter) builder.Build();

            exporter.Export();

            bool empty;

            foreach (var table in builder.GetTableInfo())
            {
                empty = string.IsNullOrWhiteSpace(File.ReadAllText(table.Name + ".json"));
                Assert.False(empty);
            }
        }

        [Theory]
        [InlineData("Area", "ReleaseGroup")]
        [InlineData("Label", "Release", "Work")]
        [InlineData("Artist", "Place", "Recording", "Url")]
        public void ExportSeveralTablesToJson_ShouldNotBeEmpty(params string [] tables)
        {
            TableToJsonExporterBuilder builder = new();

            foreach (string tableName in tables)
            {
                builder.UseTable(tableName);
            }

            var exporter = (TableToJsonExporter) builder.Build();

            exporter.Export();

            bool empty;

            foreach (var table in tables)
            {
                empty = string.IsNullOrWhiteSpace(File.ReadAllText(table + ".json"));
                Assert.False(empty);
            }
        }

        [Theory]
        [InlineData("Area", 5, 2)]
        [InlineData("Artist", 10, 1)]
        [InlineData("Label", 1, 8)]
        [InlineData("Place", 5, 5)]
        [InlineData("Recording", 10, 1)]
        [InlineData("Release", 7, 2)]
        [InlineData("ReleaseGroup", 5, 2)]
        [InlineData("Url", 4, 3)]
        [InlineData("Work", 2, 6)]
        public void ExportOnePaginatedTable_ShouldWork(string tableName, int recordsPerPage, int pageNumber)
        {
            TableToJsonExporterBuilder builder = new();

            builder.UseTable(tableName);

            builder.EnablePagination(recordsPerPage, pageNumber);

            var exporter = (TableToJsonExporter) builder.Build();

            exporter.Export();

            bool empty;

            empty = string.IsNullOrWhiteSpace(File.ReadAllText(tableName + ".json"));

            Assert.False(empty);
        }
    }
}