using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWM.Models;
using SWM.Models.Repositories;
using SWM.ViewModels;
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
        private UserManager<SwmUser> _userManager;
        private SwmContext _ctx;

        public PublicController(ISwmRepository repo, UserManager<SwmUser> userManager, SwmContext ctx)
        {
            _repo = repo;
            _userManager = userManager;
            _ctx = ctx;
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

        [Route("Public/UserDetails/{subId}")]
        public IActionResult MoreUserDetails(string subId)
        {
            ViewBag.Title = "More User Details";
            string userId = _ctx.UserToSubscriptions.FirstOrDefault(us => us.SubscriptionId == subId).UserID;
            var user = _userManager.FindByIdAsync(userId).Result;
            return View(_repo.GetDashBoardForUser(userId));
        }

        public IActionResult AdvanceSearch()
        {
            ViewBag.Title = "Advance Search";
            
            return View(new Tuple<AdvanceSearchModel, List<SearchUserModel>>(new AdvanceSearchModel(), new List<SearchUserModel>()));
        }

        public IActionResult ShowAllUsers()
        {
            ViewBag.Title = "All Users";
            return View(_repo.GetSearchResultForUserByFullName(""));
        }
    }
}
