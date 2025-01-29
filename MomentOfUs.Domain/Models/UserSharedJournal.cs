using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    /// <summary>
    /// This model is require to handle permission for each individual
    /// </summary>
    public class UserSharedJournal
    {
        [Required]
        public string UserId { get; set; }=string.Empty;
        public Guid SharedJournalId { get; set; }
        public PermissionLevel PermissionLevel { get; set; }

        public User? User { get; set; }
        public SharedJournal? SharedJournal { get; set; }
    }
        public enum PermissionLevel
    {
        View,
        Edit
    }
}