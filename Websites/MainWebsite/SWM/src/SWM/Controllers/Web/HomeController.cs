using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private SignInManager<SwmUser> _signInManager;
        private UserManager<SwmUser> _userManager;
        private SwmContext _ctx;

        public HomeController(SignInManager<SwmUser> signInManager, UserManager<SwmUser> userManager, SwmContext ctx,
            IConfigurationRoot config, IMailService mailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ctx = ctx;
            _mailService = mailService;
            _config = config;
        }

        [Route("/SignIn")]
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Sign In";
                return View();
            }
            else
                return RedirectToAction("Dashboard", "User");
        }

        [HttpPost]
        [Route("/SignIn")]
        public async Task<IActionResult> Index(LoginViewModel loginData, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                ViewBag.Title = "Sign In";
                var user = await _userManager.FindByNameAsync(loginData.UserName);
                if (user != null)
                {
                    var res = await _signInManager.PasswordSignInAsync(user.UserName, loginData.Password, loginData.Remember, false);
                    if (res.Succeeded)
                    {
                        Response.Cookies.Append("fullName", user.FullName);
                        if (string.IsNullOrWhiteSpace(returnUrl))
                            return RedirectToAction("Dashboard", "User");
                        else
                            return Redirect(returnUrl);
                    }
                    else
                        ModelState.AddModelError("", "User name or password incorrect.");
                }
                else
                    ModelState.AddModelError("", "User name or password incorrect.");
            }

            return View();
        }

        [Route("/Contact")]
        public IActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            return View();
        }

        [HttpPost]
        [Route("/Contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            string body = String.Format(System.IO.File.ReadAllText("MailBodies/_mailBodies/Contact.html"),
                model.Name,
                model.Email,
                model.Message);

            var res = _mailService.SendMail(model.Name, model.Email,_config["MailSettings:MailAddress-BhaveshJ:Name"], 
                      _config["MailSettings:MailAddress-BhaveshJ:Email"], "Contact From SWM", body);

            if (res)
            {
                ViewBag.SuccessMessage = "Message Sent!";
                ModelState.Clear();
            }
            else
                ModelState.AddModelError("", "There is a problem while sending this message. Try again after some time.");

            ViewBag.Title = "Contact Us";
            return View();
        }

        [Route("/PasswordReset")]
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

        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("/Error/{statusCode}")]
        public IActionResult StatusCodePage(string statusCode)
        {
            return View("~/Views/Home/StatusCodePages/" + statusCode + ".cshtml");
        }
    }
}
