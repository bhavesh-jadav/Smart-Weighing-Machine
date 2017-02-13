using Microsoft.AspNetCore.Mvc;
using SWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Controllers.Web
{
    public class PublicController : Controller
    {
        private ISwmRepository _repo;

        public PublicController(ISwmRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            ViewBag.Title = "Public";
            return View(_repo.GetDashBoardForPublic());
        }
    }
}
