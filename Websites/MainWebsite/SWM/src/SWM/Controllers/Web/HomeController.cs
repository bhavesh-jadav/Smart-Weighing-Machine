using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SWM.Models;
using SWM.Services;
using SWM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Controllers.Web
{
    public class HomeController : Controller
    {
        private IMailService _mailServie;
        private IConfigurationRoot _config;
        private SignInManager<SwmUser> _signInManager;
        private UserManager<SwmUser> _userManager;
        private IHostingEnvironment _env;
        private RoleManager<UserRoleManager> _roleManager;

        public HomeController(IMailService mailService, IConfigurationRoot config, 
            SignInManager<SwmUser> signInManager, UserManager<SwmUser> userManager,
            IHostingEnvironment env, RoleManager<UserRoleManager> roleManager)
        {
            _mailServie = mailService;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Sign In";
                return View();
            }
            else
                return RedirectToAction("Dashboard", "User", new { username = User.Identity.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel user, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                ViewBag.Title = "Sign In";
                var suser = await _userManager.FindByNameAsync(user.UserName);
                if (suser != null)
                {
                    //await _userManager.AddClaimAsync(suser, new Claim("Email", user.UserEmail));
                    var res = await _signInManager.PasswordSignInAsync(suser.UserName, user.Password, user.Remember, false);
                    if (res.Succeeded)
                    {
                        if (_env.IsEnvironment("Maintenance") && await _userManager.IsInRoleAsync(suser, "user"))
                            return View("Maintenance");
                        else
                        {
                            if (string.IsNullOrWhiteSpace(returnUrl))
                                return RedirectToAction("Dashboard", "User", new { username = suser.UserName });
                            else
                                return Redirect(returnUrl);
                        }
                    }
                    else
                        ModelState.AddModelError("", "User name or password incorrect.");
                }
                else
                    ModelState.AddModelError("", "User name or password incorrect.");
            }

            return View();
        }

        public async Task<IActionResult> SignOut()
        {
            if(User.Identity.IsAuthenticated)
                await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            string body = String.Format(System.IO.File.ReadAllText("MailBodies/Contact.html"),
                model.Name,
                model.Email,
                model.Message);

            var res = _mailServie.SendMail(model.Name, model.Email,_config["MailSettings:MailAddress-BhaveshJ:Name"], 
                      _config["MailSettings:MailAddress-BhaveshJ:Email"], "Contact From SWM", body);

            if (res == 1)
            {
                ViewBag.SuccessMessage = "Message Sent!";
                ModelState.Clear();
            }
            else
                ModelState.AddModelError("", "There is a problem while sending this message. Try again after some time.");

            ViewBag.Title = "Contact Us";
            return View();
        }

        public IActionResult PasswordReset()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Password Reset";
                return View();
            }
            else
                return Redirect("/");
        }
    }
}
