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
        public IActionResult AddNewMachine()
        {

            return View("~/Views/Admin/AddNewMachine.cshtml");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult AddNewMachine(AddNewMachineModel newMachine)
        {

            return View("~/Views/Admin/AddNewMachine.cshtml");
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

            var res = _repo.GetSubscriptionTypes();
            userModel = new AddNewUserModel();
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
    }
}
