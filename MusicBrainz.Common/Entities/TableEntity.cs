using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    public abstract class TableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }


    }
}
