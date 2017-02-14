using SWM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class UserDetailsModel
    {
        public int TotalProducts { get; set; }
        public long TotalWeight { get; set; }
        public int TotalLocation { get; set; }
        public List<UserDetailsLatestUpdatedTableModel> LatestUpdatedTables { get; set; }
        public List<ProductInformation> AllProducts { get; set; }
        public List<UserLocation> ALlLocations { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
    }
}
