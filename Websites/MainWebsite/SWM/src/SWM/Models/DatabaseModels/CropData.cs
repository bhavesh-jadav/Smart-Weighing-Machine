using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class CropData
    {
        [Required]
        public int CropToUserId { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public int Weight { get; set; }

        public int TroughId { get; set; }
        [Required]
        public int FarmLocationId { get; set; }
    }
}