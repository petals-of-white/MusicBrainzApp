using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Label : TableEntity
    {
        public Area? Area { get; set; }

        public short? BeginDateDay { get; set; }

        public short? BeginDateMonth { get; set; }

        public short? BeginDateYear { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        public short? EndDateDay { get; set; }

        public short? EndDateMonth { get; set; }

        public short? EndDateYear { get; set; }

        [Required]
        public bool Ended { get; set; } = false;

        [Range(1, 99999)]
        public int? LabelCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SortName { get; set; }
    }
}
