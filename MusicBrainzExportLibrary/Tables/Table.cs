namespace MusicBrainzExportLibrary.Tables
{
    public struct Table : ITable
    {
        public string Name { get; set; }

        public int? NumberOfRecords { get; set; }
    }
}
