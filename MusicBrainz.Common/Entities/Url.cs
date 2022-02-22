using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Url : TableEntity
    {
        [Required]
        [Url]
        public string UrlValue { get; set; }
    }
}
