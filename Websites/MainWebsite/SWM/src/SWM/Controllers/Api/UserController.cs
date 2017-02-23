using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SWM.Models;
using SWM.Models.Repositories;
using System.Collections.Generic;
using SWM.ViewModels;
using SWM.Models.ApiModels;
using System.Security.Claims;

namespace SWM.Controllers.Api
{
    [Route("api")]
    public class UserController : Controller
    {
        private ISwmRepository _repo;
        private UserManager<SwmUser> _userManager;
        private SignInManager<SwmUser> _signInManager;

        public UserController(ISwmRepository repo, UserManager<SwmUser> userManager, SignInManager<SwmUser> signInManager)
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("{userName}")]
        public IActionResult GetUserInfo(string userName)
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            if (user != null)
            {
                var result = _repo.GetDashBoardForUser(user.Id);
                return Ok(result);
            }
            else
                return BadRequest("invalid user");
        }

        [HttpGet("public_dashboard")]
        public IActionResult PublicDashboard()
        {
            return Ok(_repo.GetDashBoardForPublic());
        }

        [HttpGet("{userName}/product_info")]
        public IActionResult GetProductInfo(string userName)
        {
            List<ProductInfoModel> productInfos = _repo.GetProductInfoByUserName(userName);
            if (productInfos.Count > 0)
                return Ok(productInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("product_info")]
        public IActionResult GetProductInfo()
        {
            List<ProductInfoModel> productInfos = _repo.GetProductInfoByUserName("");
            if (productInfos.Count > 0)
                return Ok(productInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/product_info_month_wise/{startMonth}/{startYear}/{endMonth}/{endYear}")]
        public IActionResult GetProductInforMonthWise(string userName, int startMonth, int startYear, int endMonth, int endYear)
        {
            var res = _repo.GetProductDataMonthWise(userName, startMonth, startYear, endMonth, endYear);
            if (res.Count > 0)
                return Ok(res);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("product_info_month_wise/{startMonth}/{startYear}/{endMonth}/{endYear}")]
        public IActionResult GetProductInforMonthWise(int startMonth, int startYear, int endMonth, int endYear)
        {
            var res = _repo.GetProductDataMonthWise("", startMonth, startYear, endMonth, endYear);
            if (res.Count > 0)
                return Ok(res);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/user_dates")]
        public IActionResult UserMonths(string userName)
        {
            var res = _repo.GetUserMonths(userName);
            if (res.Count > 0)
                return Ok(res);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("user_dates")]
        public IActionResult UserMonths()
        {
            var res = _repo.GetUserMonths("");
            if (res.Count > 0)
                return Ok(res);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/location_info")]
        public IActionResult GetLocationInfo(string userName)
        {
            List<LocationInfoModel> locationInfos = _repo.GetLocationInfoByUserName(userName);
            if (locationInfos.Count > 0)
                return Ok(locationInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/{locationName}/product_info")]
        public IActionResult GetProductInfo(string userName, string locationName)
        {
            List<ProductInfoModel> productInfos = _repo.GetProductInfoByUserNameAndLocation(locationName, userName);
            if (productInfos.Count > 0)
                return Ok(productInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("add_subscirption")]
        public IActionResult AddNewSubscription([FromBody]AddNewSubscriptionModel addNewSubscription)
        {
            if (ModelState.IsValid)
            {
                return Created($"api/{addNewSubscription.UserName}", addNewSubscription);
            }
            return BadRequest("Error adding subscription");
        }

        [AllowAnonymous]
        [HttpPost("machine_data")]
        public IActionResult GetDataFromMachine([FromBody]DataFromMachineModel data)
        {
            var res = _repo.AddNewDataFromMachine(data);
            if (res)
                return Ok("success");
            else
                return BadRequest("fail");
        }

        //[HttpGet("{userName}/dashboardInfo")]
        //public IActionResult GetUserDashBoardInformation(string userName)
        //{
        //    var user = _userManager.FindByNameAsync(userName).Result;
        //    var res = _repo.GetDashBoardForUser(user.Id);
        //    return Ok(res);
        //}

        [HttpPost("advance_search")]
        public IActionResult AdvanceSearch([FromBody]AdvanceSearchModel parameters)
        {
            var result = _repo.AdvanceSearchResults(parameters);
            return Ok(result);
        }

        [HttpGet("get_all_users")]
        public IActionResult GetAllUsersForPublic()
        {
            var result = _repo.GetSearchResultForUserByFullName("");
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LogIn([FromBody]LoginViewModel user_login)
        {
            var user = _repo.GetUserByUserName(user_login.UserName);
            
            if (user.Result != null)
            {
                var res = _repo.CheckUserPassword(user.Result, user_login.Password).Result;
                if (res)
                {
                    DataForMachineModel data = new DataForMachineModel();
                    data.UserId = user.Result.Id;
                    data.FullName = user.Result.FullName;
                    data.LocationNames = _repo.GetLocationNames(user.Result.Id);
                    data.ProductNames = _repo.GetProductNames(user.Result.Id);
                    return Ok(data);
                }
                else
                    return BadRequest("fail");
            }
            else
                return BadRequest("fail");
        }
    }
}
