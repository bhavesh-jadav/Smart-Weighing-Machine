using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class AddNewMachineModel
    {
        [Required(ErrorMessage = "Machine Id required")]
        public int MachineID { get; set; }

        [Required(ErrorMessage = "Manufacture date is required")]
        public DateTime ManufacutreDate { get; set; }

        [Required(ErrorMessage = "Manufacutre location required")]
        public string ManufactureLocation { get; set; }
    }
}
