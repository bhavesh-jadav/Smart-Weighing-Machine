using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class SwmUser : IdentityUser
    {
        public SwmUser()
        {
            this.ProductToUser = new HashSet<ProductsToUser>();
            this.MachineToUser = new HashSet<MachineToUser>();
            this.UserLocation = new HashSet<UserLocation>();
        }

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

        public virtual ICollection<ProductsToUser> ProductToUser { get; set; }
        public virtual ICollection<MachineToUser> MachineToUser { get; set; }
        public virtual ICollection<UserLocation> UserLocation { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        public virtual UserToSubscription UserToSubscription { get; set; }
    }
}
