using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;
using MusicBrainzExportLibrary.Exporting;
using System.IO;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Reflection;

namespace MusicBrainzTestLibrary
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
        //[TestBeforeAfter]
        
        public void ExportOneTableToJson_ShouldNotBeEmpty(string tableName)
        {
            TableToJsonExporterBuilder builder = new();

            builder.UseTable(tableName);
          
            var exporter = (TableToJsonExporter)builder.Build();


            exporter.Export();

            bool empty = string.IsNullOrWhiteSpace( File.ReadAllText(exporter.JsonPath));

            Assert.False(empty);
        }

        [Fact]
        public void ExportAllTablesToJson_ShouldNotBeEmpty()
        {
            TableToJsonExporterBuilder builder = new();

            builder.UseAllTables();

            var exporter = (TableToJsonExporter) builder.Build();


            exporter.Export();

            bool empty = string.IsNullOrWhiteSpace(File.ReadAllText(exporter.JsonPath));

            Assert.False(empty);
        }

        [Theory]
        [InlineData("Area","ReleaseGroup")]
        [InlineData("Label", "Release","Work")]
        [InlineData("Artist", "Place", "Recording", "Url")]
        public void ExportSeveralTablesToJson_ShouldNotBeEmpty(params string[] tables)
        {
            TableToJsonExporterBuilder builder = new();

            foreach(string tableName in tables)
            {
                builder.UseTable(tableName);
            }

            var exporter = (TableToJsonExporter) builder.Build();

            exporter.Export();

            bool empty = string.IsNullOrWhiteSpace(File.ReadAllText(exporter.JsonPath));

            Assert.False(empty);

        }
    }



    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    //public class TestBeforeAfter : BeforeAfterTestAttribute
    //{
    //    public override void Before(MethodInfo methodUnderTest)
    //    {
    //        base.Before(methodUnderTest);
    //    }
    //    public override void After(MethodInfo methodUnderTest)
    //    {
    //        string path = "musicbrainz.json";

    //        Process.Start("notepad.exe", path);

    //    }
    //}

}
