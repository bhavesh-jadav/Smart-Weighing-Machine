using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Enter your name")]
        [StringLength(40, ErrorMessage = "Name must be between 3 and 40 characters", MinimumLength = 3)]
        [RegularExpression(@"[a-zA-Z''-'\s]*$", ErrorMessage = "Name can only contain letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter your email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter your message")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 10000 characters")]
        public string Message { get; set; }
    }
}
