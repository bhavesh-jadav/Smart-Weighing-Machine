using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.Models.ApiModels;

namespace SWM.Models.ApiModels
{
    public class ProductDataMonthWiseModel
    {
        public DateTime Date { get; set; }
        public List<ProductInfoModel> ProductInformation { get; set; }
    }
}
