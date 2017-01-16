using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SWM.Models;
using Microsoft.AspNetCore.Identity;
using SWM.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SWM.Models.Repositories;
using System.Security.Claims;

namespace SWM.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private SignInManager<SwmUser> _signInManager;
        private ISwmRepository _repo;

        public UserController(SignInManager<SwmUser> signInManager, ISwmRepository repo)
        {
            _signInManager = signInManager;
            _repo = repo;
        }

        /*---------------------Commen actions between all type of users------------------------------*/

        public IActionResult Dashboard()
        {
            if (User.IsInRole("admin"))
            {
                return View("AdminDashboard");
            }
            else if (User.IsInRole("user"))
            {
                return View("UserDashboard", _repo.GetDashBoardForUser(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            }
            return View();
        }

        [AllowAnonymous]
        [Route("/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            if (User.Identity.IsAuthenticated)
                await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Settings()
        {
            
            return View();
        }

        /*--------------------------------Admin actions--------------------------------------*/

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult AdminSettings(AdminSettingsModel settings)
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddNewUser()
        {
            var res = _repo.GetSubscriptionTypes();
            AddNewUserModel userModel = new AddNewUserModel();
            if (res.Length > 0)
            {
                userModel.SubscriptionTypes = res;
                return View(userModel);
            }
            else
            {
                ModelState.AddModelError("", "Unable to get subscription types");
                return View(userModel);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddNewUser(AddNewUserModel userModel)
        {
            if (_repo.AddNewUser(userModel).Result)
            {
                ViewBag.SuccessMessage = "Successfully Added New User";
                ModelState.Clear();
            }
            else
                ModelState.AddModelError("", "Unable to register new user");

            var res = _repo.GetSubscriptionTypes();
            userModel = new AddNewUserModel();
            ModelState.Clear();
            if (res.Length > 0)
            {
                userModel.SubscriptionTypes = res;
                return View(userModel);
            }
            else
            {
                ModelState.AddModelError("", "Unable to get subscription types");
                return View(userModel);
            }
        }

        [Authorize(Roles = "admin")]
        public IActionResult ShowUsers()
        {
            return View(_repo.GetAllUsers());
        }

        [Authorize(Roles = "admin")]
        public IActionResult RemoveUser()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult RemoveUser(RemoveUserModel userModel)
        {
            if (_repo.RemoveUser(userModel).Result)
            {
                ViewBag.SuccessMessage = "Successfully Removed User";
                ModelState.Clear();
            }
            else 
                ModelState.AddModelError("", "Either user name or user id is incorrect");
            return View();
        }

        /*--------------------------------User actions--------------------------------------*/

        [HttpPost]
        [Authorize(Roles = "user")]
        public IActionResult UserSettings(UserSettingsModel settings)
        {
            return View(settings);
        }

        [Authorize(Roles = "user")]
        public IActionResult ShowData()
        {
            List<TableDataModel> tableData = _repo.GetDataForDataTable(_repo.GetUserByUserId(User.FindFirst(ClaimTypes.NameIdentifier).Value).Result);
            return View(tableData);
        }

        [Authorize(Roles = "user")]
        public IActionResult AddNewLocation()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public IActionResult AddNewLocation(AddNewLocationModel newLocation)
        {
            if(_repo.AddNewLocation(newLocation, User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                ViewBag.SuccessMessage = "Successfully Added New User";
                ModelState.Clear();
            } 
            else
                ModelState.AddModelError("", "Unable to add new location. Try again later");
            return View();
        }
    }
}
