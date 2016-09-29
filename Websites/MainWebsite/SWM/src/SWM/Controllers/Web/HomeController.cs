using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SWM.Models;
using SWM.Services;
using SWM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public HomeController(IMailService mailService, IConfigurationRoot config, SignInManager<SwmUser> signInManager, UserManager<SwmUser> userManager)
        {
            _mailServie = mailService;
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewBag.Title = "Sign In";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel user, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                ViewBag.Title = "Sign In";
                var suser = await _userManager.FindByEmailAsync(user.UserEmail);
                if (suser != null)
                {
                    var res = await _signInManager.PasswordSignInAsync(suser.UserName, user.Password, user.Remember, false);
                    if (res.Succeeded)
                    {
                        if (string.IsNullOrWhiteSpace(returnUrl))
                            return RedirectToAction("Dashboard", "User");
                        else
                            return Redirect(returnUrl);
                    }
                    else
                        ModelState.AddModelError("", "Email or password incorrect.");
                }
                else
                    ModelState.AddModelError("", "Email or password incorrect.");
            }

            return View();
        }

        public async Task<IActionResult> SignOut()
        {
            if(User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }

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

            var res = _mailServie.SendMail(_config["MailSettings:MailAddress1:Name"], _config["MailSettings:MailAddress1:Email"], model.Name, model.Email, "Contact From SWM", body);
            if (res == 1)
            {
                ViewBag.SuccessMessage = "Message Sent!";
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "There is a problem while sending this message. Try again after some time.");
            }
            ViewBag.Title = "Contact Us";
            return View();
        }

        public IActionResult PasswordReset()
        {
            ViewBag.Title = "Reset Password";
            return View();
        }
    }
}
