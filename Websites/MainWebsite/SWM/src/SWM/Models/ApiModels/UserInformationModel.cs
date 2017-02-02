using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models.ApiModels
{
    public class UserInformationModel
    {
        public int TotalProducts { get; set; }
        public int TotalWeight { get; set; }
        public string SubTypeName { get; set; }
        public string ProductInforUrl { get; set; }
        public string LocationInfoUrl { get; set; }
    }
}
