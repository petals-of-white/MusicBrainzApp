using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class ReleaseGroup
    {
        [Key]
        public int Id { get; set; }

        //[Key]
        // public string Gid { get; set; }

        [Required]
        public string Name { get; set; }

        //[Required]
        //public ArtistCredit ArtistCredit { get; set; }

        //public ReleaseGroupType Type { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }


        public DateTime? LastUpdated { get; set; }
    }
}
