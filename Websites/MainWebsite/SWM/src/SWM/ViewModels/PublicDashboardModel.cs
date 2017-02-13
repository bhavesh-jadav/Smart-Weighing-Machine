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
            this.TotalMachines = this.TotalProducts = this.TotalUserLocations = this.TotalUsers = 0;
            this.TotalWeight = 0;
            this.LastUserRegisterd = "";
        }
        public int TotalUsers { get; set; }
        public long TotalWeight { get; set; }
        public int TotalProducts { get; set; }
        public int TotalMachines { get; set; }
        public int TotalUserLocations { get; set; }
        public string LastUserRegisterd { get; set; }
    }
}
