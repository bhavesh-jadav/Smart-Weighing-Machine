﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class MachineInformation
    {
        public MachineInformation()
        {
            this.MachineToUser = new HashSet<MachineToUser>();
            this.UserLocationToMachine = new HashSet<UserLocationToMachine>();
        }
        
        [Key]
        public int MachineId { get; set; }
        [Required]
        public DateTime ManufactureDate { get; set; }
        [Required]
        public bool IsAssigned { get; set; }
        [Required]
        public string ManufactureLocation { get; set; }
        public DateTime SellDate { get; set; }

        public virtual ICollection<MachineToUser> MachineToUser { get; set; }
        public virtual ICollection<UserLocationToMachine> UserLocationToMachine { get; set; }

    }
}
