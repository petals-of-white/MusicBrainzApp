using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzExportLibrary.Exporting
{
    /// <summary>
    /// Represents an exporter
    /// </summary>
    public interface IExporter
    {
        void Export();
    }
}
