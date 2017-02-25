using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class TableDataModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public long Weight { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public int MachineId { get; set; }
    }
}
