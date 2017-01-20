using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models.ApiModels
{
    public class DataFromMachineModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int weight { get; set; }
        [Required]
        public DateTime DateAndTime { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int MachineId { get; set; }
    }
}
