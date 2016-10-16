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
        [ActionName("AdminSettings")]
        public IActionResult Settings(AdminSettingsModel settings)
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult AddNewMachine()
        {

            return View();
        }

        /*--------------------------------User actions--------------------------------------*/

        [HttpPost]
        [Authorize(Roles = "user")]
        [ActionName("UserSettings")]
        public IActionResult Settings(UserSettingsModel settings)
        {
            return View("~/Views/User/Settings.cshtml", settings);
        }
    }
}
