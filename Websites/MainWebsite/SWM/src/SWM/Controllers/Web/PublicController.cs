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

        [HttpPost]
        public IActionResult SearchUser(string FullName)
        {
            ViewBag.Title = "Search User";
            var results = _repo.GetSearchResultForUserByFullName(FullName);
            ViewBag.SearchResultLength = results.Count;
            return View(results);
        }

        [Route("Public/User/{subId}")]
        public IActionResult UserDetails(string subId)
        {
            ViewBag.Title = "User Details";
            var result = _repo.GetUserDetails(subId);
            return View(result);
        }
    }
}
