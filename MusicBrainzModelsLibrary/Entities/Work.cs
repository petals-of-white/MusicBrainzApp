using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Work
    {
        [Key]
        public int Id { get; set; }

        //[Key]
        // public string Gid { get; set; }

        [Required]
        public string Name { get; set; }

        //public WorkType Type { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }


        // Not sure if I should add this one. Need to think it through.
        //public Language Language { get; set; }
    }
}
