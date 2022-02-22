using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class Recording : TableEntity
    {
        [Required]
        public string Name { get; set; }

        [Range(minimum: 0, maximum: int.MaxValue)]
        public int? Length { get; set; }

        [Required]
        public bool Video { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";
    }
}
