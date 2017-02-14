using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.ViewModels
{
    public class ShowUserModel
    {
        public int No { get; set; }
        public string UserId { get; set; }
        public string LogInUserName { get; set; }
        public string FullName { get; set; }
        public string SubscriptionType { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public DateTime DateRegisterd { get; set; }
    }
}
