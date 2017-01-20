using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class AdminDashboardModel
    {
        public AdminDashboardModel()
        {
            this.TotalLocations = this.TotalPorducts = this.TotalUsers = this.TotalWeight = 0;
        }

        public int TotalWeight { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPorducts { get; set; }
        public int TotalLocations { get; set; }
    }
}
