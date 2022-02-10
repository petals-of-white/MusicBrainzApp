namespace MusicBrainzExportLibrary.Exporting
{
    public interface IPaginationCreator
    {
        void EnablePagination(int recordsPerPage, int pageNumber);
        void DisablePagination();
    }
}
