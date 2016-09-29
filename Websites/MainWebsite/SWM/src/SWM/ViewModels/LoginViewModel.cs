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
        [Required(ErrorMessage = "Enter your email")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}
