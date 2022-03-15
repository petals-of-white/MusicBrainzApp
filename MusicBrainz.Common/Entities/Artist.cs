using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Artist : TableEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string SortName { get; set; }

        public short? BeginDateYear { get; set; }

        public short? BeginDateMonth { get; set; }

        public short? BeginDateDay { get; set; }

        public short? EndDateYear { get; set; }

        public short? EndDateMonth { get; set; }

        public short? EndDateDay { get; set; }

        [Required]
        public bool Ended { get; set; } = false;

        public Area? Area { get; set; }

        public Area? BeginArea { get; set; }

        public Area? EndArea { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";
    }
}
