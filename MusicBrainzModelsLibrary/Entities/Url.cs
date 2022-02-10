using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Url
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Url]
        public string UrlValue { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
