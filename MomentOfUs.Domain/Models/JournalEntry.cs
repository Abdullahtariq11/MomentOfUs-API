using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class JournalEntry
    {
        public Guid Id { get; set; }

        [Required]
        public Guid JournalId { get; set; }

        [Required]
        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }

        public MoodType Mood { get; set; }

        //for tracking sync
         public bool IsSynced { get; set; } = false; 

        //Navigation property
        [Required]
        [JsonIgnore]
        public Journal Journal { get; set; }

        public enum MoodType
        {
            Happy,
            Sad,
            Neutral,
            Angry,
            Stressed

        }

    }
}