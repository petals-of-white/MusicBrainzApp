using MusicBrainz.Common.Entities;

namespace MusicBrainz.DAL.Legacy
{
    public static class TablesEntitiesInfo
    {
        public static HashSet<Type> AllTypes = new()
        {
            typeof(Area),
            typeof(Artist),
            typeof(Recording),
            typeof(Label),
            typeof(Release),
            typeof(ReleaseGroup),
            typeof(Work),
            typeof(Url),
            typeof(Place),
        };
        //public enum AvailableTables : int
        //{
        //    Area,
        //    Artist,
        //    Label,
        //    Place,
        //    Recording,
        //    Release,
        //    ReleaseGroup,
        //    Url,
        //    Work
        //}

    }

}