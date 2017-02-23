using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class UserLocation
    {
        public UserLocation()
        {
            this.UserLocationToMachine = new HashSet<UserLocationToMachine>();
        }
        
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int PinNo { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CountryId { get; set; }

        public virtual ICollection<UserLocationToMachine>  UserLocationToMachine { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        [ForeignKey("UserId")]
        public virtual SwmUser SwmUser { get; set; }
    }
}
