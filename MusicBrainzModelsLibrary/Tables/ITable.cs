namespace MusicBrainzModelsLibrary.Tables
{
    public interface ITable
    {
        string Name { get; }
        int? NumberOfRecords { get; }
    }
}
