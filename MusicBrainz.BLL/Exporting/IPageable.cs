namespace MusicBrainz.BLL.Exporting
{
    public interface IPageable
    {
        int? PageNumber { get; }
        bool PaginationEnabled { get; }
        int? RecordsPerPage { get; }
    }
}
