using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MomentOfUs.Domain.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "This Information Is Required")]
        [MaxLength(10,ErrorMessage = "Character count exceeds the max limit")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "This information is Required")]
        [MaxLength(10,ErrorMessage = "Character count exceeds the max limit")]
        public string? LastName { get; set; }

        public string? ProfileImageUrl { get; set; }

        //Navigation Property
        public ICollection<Journal> Journals { get; set; }=new List<Journal>();
        public ICollection<SharedJournal> sharedJournals { get; set; }=new List<SharedJournal>();
    }
}