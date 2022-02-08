using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface IPaginationCreator
    {
        void EnablePagination(int recordsPerPage, int pageNumber);
        void DisablePagination();
    }
}
