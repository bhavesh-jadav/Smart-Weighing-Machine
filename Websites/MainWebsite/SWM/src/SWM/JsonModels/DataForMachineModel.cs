using SWM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.JsonModels
{
    public class DataForMachineModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public List<KeyValuePair<int, string>> LocationNames { get; set; }
        public List<KeyValuePair<int, string>> ProductNames { get; set; }
    }
}
