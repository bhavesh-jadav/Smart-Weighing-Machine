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
            this.TotalLocation = this.TotalProducts = 0;
            this.TotalWeight = 0;
            this.LastUpdatedProduct = "none";
            this.TotalProducts = 0;
            this.ProductsIntoAccount = "";
            this.TotalWeight = 0;
            this.UserName = "";
            this.IsNewUser = false;
            this.HaveSomeData = true;
        }
        public List<AddNewLocationModel> UserLocations { get; set; }
        public long TotalWeight { get; set; }
        public int TotalProducts { get; set; }
        public int TotalLocation { get; set; }
        public string LastUpdatedProduct { get; set; }
        public string ProductsIntoAccount { get; set; }
        public string UserName { get; set; }
        public bool IsNewUser { get; set; }
        public bool HaveSomeData { get; set; }
    }
}
