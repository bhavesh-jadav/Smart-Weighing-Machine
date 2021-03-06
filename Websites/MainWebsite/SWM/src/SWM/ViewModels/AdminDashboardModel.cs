﻿using System;
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
            this.TotalLocations = this.TotalPorducts = this.TotalUsers = 0;
            this.TotalWeight = 0;
        }

        public long TotalWeight { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPorducts { get; set; }
        public int TotalLocations { get; set; }
    }
}
