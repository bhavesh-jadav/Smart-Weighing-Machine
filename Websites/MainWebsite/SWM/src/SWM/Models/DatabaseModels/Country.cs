using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class Country
    {
        public Country()
        {
            this.SwmUser = new HashSet<SwmUser>();
            this.UserLocation = new HashSet<UserLocation>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<SwmUser> SwmUser { get; set; }
        public virtual ICollection<UserLocation> UserLocation { get; set; }
    }
}
