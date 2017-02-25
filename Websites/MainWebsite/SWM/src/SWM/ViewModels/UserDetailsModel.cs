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
            this.TotalLocation = this.TotalProducts = 0;
            this.LastUpdatedProduct = "none";
            this.ContactNo = this.Email = this.SubId = this.UserName = "";
            this.LatestUpdatedProductsTableInformation = new List<UserDetailsLatestUpdatedTableModel>();
            this.ProductsIntoAccount = new List<ProductInformation>();
            this.UserLocations = new List<AddNewLocationModel>();
            this.IsNewUser = false;
            this.HaveSomeData = true;
        }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public int TotalProducts { get; set; }
        public int TotalLocation { get; set; }
        public string LastUpdatedProduct { get; set; }
        public string SubType { get; set; }
        public long TotalWeight { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string SubId { get; set; }
        public bool IsNewUser { get; set; }
        public bool HaveSomeData { get; set; }
        public List<UserDetailsLatestUpdatedTableModel> LatestUpdatedProductsTableInformation { get; set; }
        public List<ProductInformation> ProductsIntoAccount { get; set; }
        public List<AddNewLocationModel> UserLocations { get; set; }
    }
}
