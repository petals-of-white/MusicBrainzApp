using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzDataAcessLibrary
{
    public enum AvailableTables: int
    {
        Area = 1,
        Artist,
        Label,
        Place,
        Recording,
        Release,
        ReleaseGroup,
        Url,
        Work
    }
}
