using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class Journal
    {
        public Guid JournalId { get; set; }

        [Required]
        public string? UserID { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Character count exceeds the max length.")]
        public string? Title { get; set; }

        public string? Content { get; set; } = string.Empty;

        public string? ModelBuilder { get; set; }

        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}