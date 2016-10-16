using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWM.JsonModels;
using SWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Controllers.Api
{
    [Route("api")]
    [Authorize]
    public class UserController : Controller
    {
        private ISwmRepository _repo;

        public UserController(ISwmRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{userName}")]
        public IActionResult GetUserInfo(string userName)
        {
            if (User.IsInRole("user"))
            {
                var userInformation = _repo.GetUserInformationForAPI(userName);
                if(userInformation.TotalProducts > 0)
                {
                    userInformation.ProductInforUrl = $"{Request.Host}/api/{userName}/product_info";
                    userInformation.LocationInfoUrl = $"{Request.Host}/api/{userName}/location_info";
                    return Ok(userInformation);
                }
                else
                    return BadRequest("Unable to process requrest");
            }
            return View();
        }

        [HttpGet("{userName}/product_info")]
        public IActionResult GetProductInfo(string userName)
        {
            List<ProductInfo> productInfos = _repo.GetProductInfoByUserName(userName);
            if (productInfos.Count > 0)
                return Ok(productInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/location_info")]
        public IActionResult GetLocationInfo(string userName)
        {
            List<LocationInfo> locationInfos = _repo.GetLocationInfoByUserName(userName);
            if (locationInfos.Count > 0)
                return Ok(locationInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [HttpGet("{userName}/{locationName}/product_info")]
        public IActionResult GetProductInfo(string userName, string locationName)
        {
            List<ProductInfo> productInfos = _repo.GetProductInfoByUserNameAndLocation(locationName, userName);
            if (productInfos.Count > 0)
                return Ok(productInfos);
            else
                return BadRequest("Unable to process requrest");
        }

        [Authorize(Roles = "admin")]
        [HttpPost("add_subscirption")]
        public IActionResult AddNewSubscription([FromBody]AddNewSubscription addNewSubscription)
        {
            if (ModelState.IsValid)
            {
                return Created($"api/{addNewSubscription.UserName}", addNewSubscription);
            }
            return BadRequest("Error adding subscription");
        }
    }
}
