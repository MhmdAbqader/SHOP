using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullUserName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; } = "Assuit";
        [DefaultValue("Male")]
        [Required]
        public string Gender { get; set; } 

    }
}
