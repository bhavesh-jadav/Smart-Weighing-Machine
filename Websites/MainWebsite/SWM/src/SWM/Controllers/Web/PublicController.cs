using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWM.Models;
using SWM.Models.Repositories;
using SWM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            return View();
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
            var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value).Result;
            bool isNewUser = !(_ctx.ProductsToUsers.FirstOrDefault(pu => pu.UserId == user.Id) != null && _ctx.UserLocations.FirstOrDefault(u => u.UserId == user.Id) != null);
            var ptou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
            bool haveSomeData = (_ctx.CropDatas.Where(cd => ptou.Any(c => cd.CropToUserId == c.Id)).ToList().Count > 0);
            return View(new Tuple<bool, bool, string>(isNewUser, haveSomeData, user.UserName));
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
