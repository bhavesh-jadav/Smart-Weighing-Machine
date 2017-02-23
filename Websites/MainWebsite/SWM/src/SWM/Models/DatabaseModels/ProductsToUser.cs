using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class ProductsToUser
    {
        public ProductsToUser()
        {
            this.CropData = new HashSet<CropData>();
        }

        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ProductId { get; set; }

        public virtual ICollection<CropData> CropData { get; set; }

        [ForeignKey("UserId")]
        public virtual SwmUser SwmUser { get; set; }
        [ForeignKey("ProductId")]
        public virtual ProductInformation ProductInformation { get; set; }

    }
}
