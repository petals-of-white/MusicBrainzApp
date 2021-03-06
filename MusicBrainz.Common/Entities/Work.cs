using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Work : TableEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";
    }
}