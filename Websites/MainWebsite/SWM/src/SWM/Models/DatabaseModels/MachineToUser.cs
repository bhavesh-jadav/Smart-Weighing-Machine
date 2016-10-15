using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class MachineToUser
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public int MachineId { get; set; }
    }
}
