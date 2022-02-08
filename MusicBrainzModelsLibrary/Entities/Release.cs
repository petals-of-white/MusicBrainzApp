using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Release
    {
        [Key]
        public int Id { get; set; }

        //[Key]
        // public string Gid { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public ReleaseGroup ReleaseGroup { get; set; }

        //[Required]
        //public ArtistCredit ArtistCredit { get; set; }

        [MaxLength(255)]
        public string? Barcode { get; set; }

        //public Status? Status { get; set; }
        //public Packaging? Status { get; set; }
        //public Language? Language { get; set; }
        //public Script? Script { get; set; }

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
