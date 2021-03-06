﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class SubscriptionType
    {
        public SubscriptionType()
        {
            this.UserToSubscription = new HashSet<UserToSubscription>();
        }
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<UserToSubscription> UserToSubscription { get; set; }
    }
}
