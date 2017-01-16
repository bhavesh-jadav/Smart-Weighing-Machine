using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class RemoveUserModel
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
    }
}
