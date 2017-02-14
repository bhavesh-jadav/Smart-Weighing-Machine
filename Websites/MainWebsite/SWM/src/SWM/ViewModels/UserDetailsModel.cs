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
        public UserDetailsModel()
        {
            this.TotalWeight = 0;
            this.ContactNo = this.Email = "";
            this.LatestUpdatedTables = new List<UserDetailsLatestUpdatedTableModel>();
            this.ProductsIntoAccount = new List<ProductInformation>();
            this.UserLocations = new List<AddNewLocationModel>();
        }

        public string FullName { get; set; }
        public string SubType { get; set; }
        public long TotalWeight { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public List<UserDetailsLatestUpdatedTableModel> LatestUpdatedTables { get; set; }
        public List<ProductInformation> ProductsIntoAccount { get; set; }
        public List<AddNewLocationModel> UserLocations { get; set; }
    }
}
