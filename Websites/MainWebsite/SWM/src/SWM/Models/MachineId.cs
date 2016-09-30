using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class MachineId
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public bool IsAssigned { get; set; }
    }
}
