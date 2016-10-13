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

        public async Task<IActionResult> SignOut()
        {
            if (User.Identity.IsAuthenticated)
                await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Settings()
        {
            if (User.IsInRole("admin"))
            {
                AdminSettings settings = new AdminSettings();

                var envs = _repo.GetAllEnvironments();
                List<SelectListItem> webenvs = new List<SelectListItem>();
                foreach(var env in envs)
                {
                    webenvs.Add(new SelectListItem()
                    {
                        Value = env.EnvName,
                        Text = env.EnvName
                    });
                }
                settings.WebEnvironments = webenvs;
                ViewBag.CurrentEnvironment = settings.CurrentWebEnvironMent = _repo.GetCurrentWebEnvironment();

                return View("~/Views/Admin/Settings.cshtml", settings);
            }
            else if (User.IsInRole("user"))
            {
                return View("~/Views/User/Settings.cshtml");
            }

            return View();
        }

        /*--------------------------------Admin actions--------------------------------------*/

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Settings(AdminSettings settings)
        {
            if (_repo.SetCurrentWebEnvironment(settings.CurrentWebEnvironMent))
                ViewBag.Message = "Successfully changed the settings";
            else
                ViewBag.Message = "There is an error saving the settings. Try again after some time";

            return View("~/Views/Admin/Settings.cshtml", settings);
        }



        /*--------------------------------User actions--------------------------------------*/

        [HttpPost]
        [Authorize(Roles = "user")]
        public IActionResult Settings(UserSettings settings)
        {
            return View("~/Views/User/Settings.cshtml", settings);
        }
    }
}
