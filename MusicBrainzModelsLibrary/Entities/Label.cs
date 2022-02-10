﻿using System.ComponentModel.DataAnnotations;

namespace MusicBrainzModelsLibrary.Entities
{
    public class Label
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SortName { get; set; }

        [Range(1, 99999)]
        public int? LabelCode { get; set; }

        public Area? Area { get; set; }

        [Required]
        [MaxLength(255)]
        public string Comment { get; set; } = "";

        public short? BeginDateYear { get; set; }
        public short? BeginDateMonth { get; set; }
        public short? BeginDateDay { get; set; }
        public short? EndDateYear { get; set; }
        public short? EndDateMonth { get; set; }
        public short? EndDateDay { get; set; }

        [Required]
        public bool Ended { get; set; } = false;

        [Required]
        [Range(0, int.MaxValue)]
        public int EditsPending { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
