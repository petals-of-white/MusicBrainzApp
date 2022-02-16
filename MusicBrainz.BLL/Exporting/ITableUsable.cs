namespace MusicBrainz.BLL.Exporting
{
    public interface ITableUsable
    {
        void UseTable(string table);
        void UseAllTables();
    }
}
