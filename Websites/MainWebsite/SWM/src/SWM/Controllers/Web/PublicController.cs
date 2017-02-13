using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Controllers.Web
{
    public class PublicController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Public";
            return View();
        }
    }
}
