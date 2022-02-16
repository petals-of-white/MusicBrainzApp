namespace MusicBrainz.BLL.Exporting
{
    public interface IPaginationCreator
    {
        void EnablePagination(int recordsPerPage, int pageNumber);
        void DisablePagination();
    }
}
