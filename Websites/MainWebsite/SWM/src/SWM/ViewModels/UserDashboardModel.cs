﻿using System;
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
            this.TotalLocation = this.TotalProducts = this.TotalWeight = 0;
            this.LastUpdatedProduct = "none";
        }
        public List<AddNewLocationModel> UserLocations { get; set; }
        public int TotalWeight { get; set; }
        public int TotalProducts { get; set; }
        public int TotalLocation { get; set; }
        public string LastUpdatedProduct { get; set; }
    }
}