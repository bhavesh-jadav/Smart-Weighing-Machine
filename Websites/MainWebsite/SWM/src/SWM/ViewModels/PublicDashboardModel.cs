using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class PublicDashboardModel
    {
        public PublicDashboardModel()
        {
            this.TotalMachines = this.TotalProducts = this.TotalUserLocations = this.TotalUsers = this.TotalWeight = 0;
        }
        public int TotalUsers { get; set; }
        public int TotalWeight { get; set; }
        public int TotalProducts { get; set; }
        public int TotalMachines { get; set; }
        public int TotalUserLocations { get; set; }
    }
}
