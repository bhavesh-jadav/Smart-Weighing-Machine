using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class SwmUser : IdentityUser
    {
        [StringLength(200)]
        [Required]
        public string FullName { get; set; }
    }
}
