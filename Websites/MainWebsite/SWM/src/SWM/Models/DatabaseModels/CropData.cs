using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class CropData
    {
        [Required]
        public int ProductToUserId { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public int UserLocationToMachineId { get; set; }

        [ForeignKey("ProductToUserId")]
        public virtual ProductsToUser ProductsToUser { get; set; }
        [ForeignKey("UserLocationToMachineId")]
        public virtual UserLocationToMachine UserLocationToMachine { get; set; }

    }
}