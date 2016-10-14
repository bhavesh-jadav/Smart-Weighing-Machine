using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter your user name")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "User name is either too short or too long")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [MinLength(8, ErrorMessage = "Password Must be atleast 8 characters long")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
