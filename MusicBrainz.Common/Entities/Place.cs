using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Place : TableEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; } = "";

        public Area? Area { get; set; }
        public string? Coordinates { get; set; }
        //public SqlGeography Coordinates { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        public short? BeginDateYear { get; set; }

        public short? BeginDateMonth { get; set; }

        public short? BeginDateDay { get; set; }

        public short? EndDateYear { get; set; }

        public short? EndDateMonth { get; set; }

        public short? EndDateDay { get; set; }

        [Required]
        public bool Ended { get; set; } = false;

    }
}
