using SWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class AddNewUserModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Full name must be inbetween 4 to 50 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Subscription type is required")]
        public string[] SubscriptionTypes { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Enter valid Contact number")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200,MinimumLength = 5, ErrorMessage = "Address must be inbetween 5 to 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Pin number is required")]
        [MinLength(6, ErrorMessage = "Pin number is short")]
        [MaxLength(6, ErrorMessage = "Pin number is long")]
        public string PinNo { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "State must be inbetween 3 to 50 characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Country must be inbetween 3 to 50 characters")]
        public string Country { get; set; }
    }
}
