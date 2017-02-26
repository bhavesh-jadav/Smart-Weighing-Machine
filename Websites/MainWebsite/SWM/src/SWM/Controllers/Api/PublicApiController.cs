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
    public class PublicApiController : Controller
    {
        private ISwmRepository _repo;
        private UserManager<SwmUser> _userManager;
        private SignInManager<SwmUser> _signInManager;
        private SwmContext _ctx;

        public PublicApiController(ISwmRepository repo, UserManager<SwmUser> userManager, SignInManager<SwmUser> signInManager, SwmContext ctx)
        {
            _repo = repo;
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
        }

        [HttpGet("{userName}")]
        public IActionResult GetUserInfo(string userName)
        {
            var subId = _repo.GetSubIdFromUserName(userName);
            return Ok(_repo.GetUserDetails(subId));
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
            if (productInfos != null)
            {
                if (productInfos.Count > 0)
                    return Ok(productInfos);
                else
                    return BadRequest("This user does not have any data");
            }
            else
                return BadRequest("Unable to fetch data");
        }

        [HttpGet("product_info")]
        public IActionResult GetProductInfo()
        {
            List<ProductInfoModel> productInfos = _repo.GetAllProductInformation();
            if (productInfos != null)
            {
                if (productInfos.Count > 0)
                    return Ok(productInfos);
                else
                    return BadRequest("No data");
            }
            else
                return BadRequest("Unable to fetch data");
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

        [HttpPost("advance_search")]
        public IActionResult AdvanceSearch([FromBody]AdvanceSearchModel parameters)
        {
            var result = _repo.AdvanceSearchResults(parameters);
            return Ok(result);
        }

        [HttpGet("get_all_users/{pageNo:int}")]
        public IActionResult GetAllUsersForPublic(int pageNo)
        {
            var result = _repo.GetAllUsers(pageNo);
            return Ok(result);
        }

        [HttpGet("subscription/{subId}")]
        public IActionResult GetUserDetails(string subId)
        {
            var result = _repo.GetUserDetails(subId);
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
    }
}
