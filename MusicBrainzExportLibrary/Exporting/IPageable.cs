using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface IPageable
    {
        int? PageNumber { get; }
        bool PaginationEnabled { get; }
        int? RecordsPerPage { get; }
    }
}
