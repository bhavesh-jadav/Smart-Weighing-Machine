using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.JsonModels
{
    public class AddNewSubscription
    {
        [Required(ErrorMessage = "Enter user name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter email")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Select subscription type")]
        public string SubType { get; set; }
        [Required(ErrorMessage = "Enter phone number")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "Enter proper phone number")]
        public string PhoneNumber { get; set; }
    }
}
