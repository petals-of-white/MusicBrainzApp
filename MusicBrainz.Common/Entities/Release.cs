using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Release : TableEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public ReleaseGroup ReleaseGroup { get; set; }

        [MaxLength(255)]
        public string? Barcode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        [Required]
        public short Quality { get; set; } = -1;
    }
}