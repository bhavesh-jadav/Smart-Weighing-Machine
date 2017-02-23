using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class UserLocationToMachine
    {
        public UserLocationToMachine()
        {
            this.CropData = new HashSet<CropData>();
        }

        public int Id { get; set; }
        [Required]
        public int UserLocationId { get; set; }
        [Required]
        public int MachineId { get; set; }

        public virtual ICollection<CropData> CropData { get; set; }

        [ForeignKey("UserLocationId")]
        public virtual UserLocation UserLocation { get; set; }
        [ForeignKey("MachineId")]
        public virtual MachineInformation MachineInformation { get; set; }
    }
}
