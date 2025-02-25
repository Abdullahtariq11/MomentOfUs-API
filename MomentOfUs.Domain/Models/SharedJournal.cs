using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MomentOfUs.Domain.Models
{
    public class SharedJournal
    {

        public Guid Id { get; set; }
        public string OwnerId { get; set; } =string.Empty;
        [Required]
        public Guid JournalId { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        //Navigation property
        [JsonIgnore]
        public User? Owner { get; set; }
        [JsonIgnore]
        public Journal? Journal { get; set; }
        [JsonIgnore]
        public ICollection<UserSharedJournal> SharedWith { get; set; }=new List<UserSharedJournal>();
    }

}