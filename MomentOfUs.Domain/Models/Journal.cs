using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class Journal
    {
        public Guid Id { get; set; }

        [Required]
        public string OwnerID { get; set; }=string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Character count exceeds the max length.")]
        public string? Title { get; set; }

        //Cover photos
        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; }=  DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        //Navigation Property
        [Required]
        public User? Owner { get; set; }
        public ICollection<SharedJournal> sharedJournals { get; set; }= new List<SharedJournal>();
        public ICollection<JournalEntry> journalEntries{ get; set; }=new List<JournalEntry>();

    }
}