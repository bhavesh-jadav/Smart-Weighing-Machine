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
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SWM.Controllers.Web
{
    [Authorize]
    public class UserController : Controller
    {
        private SignInManager<SwmUser> _signInManager;
        private ISwmRepository _repo;
        private IConfigurationRoot _config;
        private SwmContext _ctx;
        private UserManager<SwmUser> _userManager;

        public UserController(SignInManager<SwmUser> signInManager, ISwmRepository repo, IConfigurationRoot config, SwmContext ctx, UserManager<SwmUser> userManager)
        {
            _signInManager = signInManager;
            _repo = repo;
            _config = config;
            _ctx = ctx;
            _userManager = userManager;
        }

        /*---------------------Commen actions between all type of users------------------------------*/

        public virtual IActionResult Dashboard()
        {
            if (User.IsInRole("admin"))
            {
                return View("AdminDashboard", _repo.GetDashBoardForAdmin());
            }
            else if (User.IsInRole("user") || User.IsInRole("testuser"))
            {
                var user = _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value).Result;
                bool isNewUser = !(_ctx.ProductsToUsers.FirstOrDefault(pu => pu.UserId == user.Id) != null && _ctx.UserLocations.FirstOrDefault(u => u.UserId == user.Id) != null);
                var ptou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                bool haveSomeData = (_ctx.CropDatas.Where(cd => ptou.Any(c => cd.ProductToUserId == c.Id)).ToList().Count > 0);
                return View("UserDashboard", new Tuple<bool, bool, string>(isNewUser, haveSomeData, user.UserName));
            }
            return View();
        }

        [AllowAnonymous]
        [Route("/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
                Response.Cookies.Delete("fullName");
                if (HttpContext.Session.GetString("useraccess") != "")
                {
                    Response.Cookies.Delete("useraccess");
                    HttpContext.Session.SetString("useraccess", "");
                }
            }

            return RedirectToAction("Index", "Public");
        }

        public IActionResult Settings()
        { 
            return View();
        }

        /*--------------------------------Admin Actions--------------------------------------*/

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

        private string encrypt(string encryptString)
        {
            string EncryptionKey = _config["key"];
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        private string decrypt(string cipherText)
        {
            string EncryptionKey = _config["key"];
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        [Route("/User/{userName}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UserAcess(string userName)
        {
            var user = _repo.GetUserByUserName(userName).Result;
            if(user != null)
            {
                await SignOut();
                await _signInManager.SignInAsync(user, false, null);
                Response.Cookies.Append("fullName", user.FullName);
                string ctext = encrypt(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                HttpContext.Session.SetString("useraccess", ctext);
                Response.Cookies.Append("useraccess", ctext);
            }
            return RedirectToAction("Dashboard", "User");
        }
        /*---------------------------Admin User Access Actions------------------------------*/

        private bool isAllowed()
        {
            if (HttpContext.Session.GetString("useraccess") == "" && string.IsNullOrEmpty(HttpContext.Request.Cookies["useraccess"]))
                return false;
            return true;
        }
        
        public IActionResult AddNewLocation()
        {
            if (isAllowed())
                return View();
            else
                return RedirectToAction("AccessDenied", "Home");
        }
        
        [HttpPost]
        public IActionResult AddNewLocation(AddNewLocationModel newLocation)
        {
            if (isAllowed())
            {
                if (_repo.AddNewLocation(newLocation, User.FindFirst(ClaimTypes.NameIdentifier).Value))
                {
                    ViewBag.SuccessMessage = "Successfully Added New Location";
                    ModelState.Clear();
                }
                else
                    ModelState.AddModelError("", "Unable to add new location. Try again later");
                return View();
            }
            else
                return RedirectToAction("AccessDenied", "Home");     
        }
        
        public IActionResult AddNewProduct()
        {
            if (isAllowed())
                return View();
            else
                return RedirectToAction("AccessDenied", "Home");
        }
        
        [HttpPost]
        public IActionResult AddNewProduct(AddNewProductModel newProduct)
        {
            if (isAllowed())
            {
                if (_repo.AddNewProduct(User.FindFirst(ClaimTypes.NameIdentifier).Value, newProduct))
                {
                    ViewBag.SuccessMessage = "Successfully Added New Product";
                    ModelState.Clear();
                }
                else
                    ModelState.AddModelError("", "Unable to add new product. Try again later");
                return View();
            }
            else
                return RedirectToAction("AccessDenied", "Home");

        }

        /*--------------------------------User Actions--------------------------------------*/

        //[HttpPost]
        //[Authorize(Roles = "user")]
        //public IActionResult UserSettings(UserSettingsModel settings)
        //{
        //    return View(settings);
        //}

        [Authorize(Roles = "user,testuser")]
        public IActionResult ShowData()
        {
            List<TableDataModel> tableData = _repo.GetDataForDataTable(_repo.GetUserByUserId(User.FindFirst(ClaimTypes.NameIdentifier).Value).Result);
            return View(tableData);
        }


    }
}
