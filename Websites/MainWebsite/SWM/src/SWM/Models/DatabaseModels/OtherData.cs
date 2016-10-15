using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class OtherData
    {
        [Key]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
