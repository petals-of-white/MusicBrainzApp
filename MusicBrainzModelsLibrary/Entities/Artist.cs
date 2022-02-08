﻿using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        //[Key]
        // public string Gid { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SortName { get; set; }

        //public ArtistType Type { get; set; }


        public short? BeginDateYear { get; set; }
        public short? BeginDateMonth { get; set; }
        public short? BeginDateDay { get; set; }
        public short? EndDateYear { get; set; }
        public short? EndDateMonth { get; set; }
        public short? EndDateDay { get; set; }


        // public Gender? Gender { get; set; }

        [Required]
        public bool Ended { get; set; } = false;

        public Area? Area { get; set; }
        public Area? BeginArea { get; set; }
        public Area? EndArea { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";


        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }

    }
}