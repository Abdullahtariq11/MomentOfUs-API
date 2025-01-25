using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class SharedJournal
    {
        public Guid SharedJournalId { get; set; }
        public string? OwnerId { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
        public string? SharedWithId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }

    public enum PermissionLevel
    {
        view,
        edit
    }
}