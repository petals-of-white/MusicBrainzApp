using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public class ReleaseGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
