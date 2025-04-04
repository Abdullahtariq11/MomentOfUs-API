using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MomentOfUs.Domain.Models;

namespace MomentOfUs.Application.Dtos.SharedJournalDto
{
    public class JournalSharedWithMeDto
    {
        public Guid SharedJournalId { get; set; }
        public Guid JournalId { get; set; }
        public string Title { get; set; } = default!;
        public string? PhotoUrl { get; set; }
        public string OwnerId { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
    }
}
