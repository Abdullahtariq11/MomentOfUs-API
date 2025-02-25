using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    /// <summary>
    /// This model is require to handle permission for each individual
    /// </summary>
    public class UserSharedJournal
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }=string.Empty;
        
        public Guid SharedJournalId { get; set; }
        public PermissionLevel PermissionLevel { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public SharedJournal? SharedJournal { get; set; }
    }
        public enum PermissionLevel
    {
        View,
        Edit
    }
}