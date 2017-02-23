using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("UserID")]
        public virtual SwmUser SwmUser { get; set; }
        [ForeignKey("MachineId")]
        public MachineInformation MachineInformation { get; set; }
    }
}
