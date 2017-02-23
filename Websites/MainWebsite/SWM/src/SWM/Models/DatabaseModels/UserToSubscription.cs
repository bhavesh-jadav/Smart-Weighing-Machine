using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string SubscriptionId { get; set; }
        [Required]
        public int SubscriptionTypeId { get; set; }

        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }
        [ForeignKey("UserID")]
        public virtual SwmUser SwmUser { get; set; }
    }
}
