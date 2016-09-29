using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class UserToSubscription
    {
        [Required]
        public string UserID { get; set; }
        [Required]
        public int SubscriptionId { get; set; }
        [Required]
        public int SubscriptionTypeId { get; set; }
    }
}
