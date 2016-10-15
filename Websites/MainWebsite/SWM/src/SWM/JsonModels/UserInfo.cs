using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.JsonModels
{
    public class UserInfo
    {
        public int TotalProducts { get; set; }
        public int TotalWeight { get; set; }
        public string SubType { get; set; }
        public string ProductInfoUrl { get; set; }
        public string LocationInfoUrl { get; set; }
    }
}
