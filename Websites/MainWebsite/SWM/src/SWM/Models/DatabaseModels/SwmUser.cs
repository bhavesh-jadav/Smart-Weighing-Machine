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
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        public int PinNo { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public DateTime RegisterDate { get; set; }
    }
}
