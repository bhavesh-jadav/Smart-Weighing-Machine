using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class AddNewProductModel
    {
        public AddNewProductModel()
        {
            this.Name = "";
            this.Description = "";
        }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength =3, ErrorMessage = "Product name must be between 3 to 100 characters")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
