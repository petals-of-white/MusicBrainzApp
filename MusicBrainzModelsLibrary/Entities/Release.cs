using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Release
    {
        [Key]
        public int Id { get; set; }

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
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        [Required]
        public short Quality { get; set; } = -1;

        public DateTime? LastUpdated { get; set; }
    }
}
