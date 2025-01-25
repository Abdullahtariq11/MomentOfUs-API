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
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "This information is Required")]
        public string? LastName { get; set; }
    }
}