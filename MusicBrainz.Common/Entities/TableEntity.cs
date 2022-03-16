using System.ComponentModel.DataAnnotations;

namespace MusicBrainz.Common.Entities
{
    /// <summary>
    /// This class is abstract. It provides basic information about table entities.
    /// </summary>
    public abstract class TableEntity
    {
        //[System.ComponentModel.DataAnnotations.Key]
        //[Dapper.Contrib.Extensions.Key]
        //[Write(false)]
        public int Id { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
