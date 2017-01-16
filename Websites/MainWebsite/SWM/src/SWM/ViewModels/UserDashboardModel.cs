using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class UserDashboardModel
    {
        public UserDashboardModel()
        {
            this.UserLocations = new List<AddNewLocationModel>();
        }
        public List<AddNewLocationModel> UserLocations { get; set; }
    }
}
