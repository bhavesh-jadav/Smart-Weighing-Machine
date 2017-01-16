using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Services
{
    public interface IMailService
    {
        bool SendMail(string fromName, string from, string toName, string to, string subject, string body);
    }
}
