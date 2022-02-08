using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicBrainzModelsLibrary.Tables;

namespace MusicBrainzExportLibrary.Exporting
{
    public interface ITableSelector
    {
        IList<ITable> TablesInfo { get; }
        IList<ITable> SelectedTables { get; }
    }
    
}
