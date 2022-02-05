using System.ComponentModel.DataAnnotations;

namespace MusicBrainzDataAcessLibrary.Entities
{
    public class Recording
    {
        [Key]
        public int Id { get; set; }

        //[Key]
        // public string Gid { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required]
        //public ArtistCredit ArtistCredit { get; set; }
        [Range(minimum: 0, maximum: int.MaxValue)]
        public int? Length { get; set; }

        [Required]
        public bool Video { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }


        public DateTime? LastUpdated { get; set; }
    }
}
