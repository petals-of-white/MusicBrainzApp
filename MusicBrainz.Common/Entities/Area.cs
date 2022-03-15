using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Area : TableEntity
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

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";
    }
}