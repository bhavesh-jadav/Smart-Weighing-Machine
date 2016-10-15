using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.JsonModels;
using Microsoft.AspNetCore.Identity;

namespace SWM.Models.Repositories
{
    public class SwmRepository : ISwmRepository
    {
        private SwmContext _ctx;
        private UserManager<SwmUser> _userManager;

        public SwmRepository(SwmContext ctx, UserManager<SwmUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }
        public string GetCountryName(int countryId)
        {
            return _ctx.Countries.FirstOrDefault(c => c.Id == countryId).Name;
        }
        public int GetPinNumber(int pinId)
        {
            return _ctx.PinNumbers.FirstOrDefault(p => p.Id == pinId).Pin;
        }
        public string GetStateName(int stateId)
        {
            return _ctx.States.FirstOrDefault(s => s.Id == stateId).Name;
        }
        public List<LocationInfo> GetLocationInfoByUserName(string userName)
        {
            try
            {
                List<LocationInfo> locationInfos = new List<LocationInfo>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    UserLocation[] farmLocations = _ctx.UserLocations.Where(f => f.UserId == user.Id).ToArray();
                    foreach (var floc in farmLocations)
                    {
                        locationInfos.Add(new LocationInfo()
                        {
                            Name = floc.Name,
                            Address = floc.Address + ", " + GetStateName(floc.StateId) + ", " + GetCountryName(floc.CountryId) + ", " +
                                      "Pin Number: " + GetPinNumber(floc.PinId).ToString()
                        });
                    }
                    return locationInfos;
                }
                else
                    return new List<LocationInfo>();

            }
            catch (Exception ex)
            {
                return new List<LocationInfo>();
            }
        }
        public List<ProductInfo> GetProductInfoByUserName(string userName)
        {
            try
            {
                List<ProductInfo> productInfo = new List<ProductInfo>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    ProductsToUser[] ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                    foreach(var cu in ctou)
                    {
                        productInfo.Add(new ProductInfo()
                        {
                            ProductName = GetProductInformation(cu.ProductID).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.CropToUserId == cu.Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return productInfo;
                }
                else
                {
                    return new List<ProductInfo>();
                }
            }
            catch (Exception ex)
            {
                return new List<ProductInfo>();
            }
        }
        public async Task<SwmUser> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public List<ProductInfo> GetProductInfoByUserNameAndLocation(string locationName, string userName)
        {
            try
            {
                List<ProductInfo> productInfos = new List<ProductInfo>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    ProductsToUser[] ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                    foreach (var cu in ctou)
                    {
                        productInfos.Add(new ProductInfo()
                        {
                            ProductName = GetProductInformation(cu.ProductID).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.CropToUserId == cu.Id && cd.FarmLocationId == _ctx.UserLocations.FirstOrDefault(ul => ul.Name == locationName && ul.UserId == user.Id).Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return productInfos;
                }
                else
                    return new List<ProductInfo>();
            }
            catch (Exception ex)
            {
                return new List<ProductInfo>();
            }
        }
        public ProductInformation GetProductInformation(int productId)
        {
            return _ctx.ProductInformations.FirstOrDefault(p => p.Id == productId);
        }
        public UserInformation GetUserInformationForAPI(string userName)
        {
            try
            {
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    UserInformation userInformation = new UserInformation();
                    userInformation.TotalProducts = _ctx.ProductsToUsers.Where(p => p.UserId == user.Id).Count();

                    List<ProductInfo> productInfo = GetProductInfoByUserName(userName);
                    foreach (var pinfo in productInfo)
                        userInformation.TotalWeight += pinfo.TotalWeight;

                    userInformation.SubTypeName = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Id == _ctx.UserToSubscriptions.FirstOrDefault(u => u.UserID == user.Id).SubscriptionTypeId).Name;

                    return userInformation;
                }
                else
                    return new UserInformation();
            }
            catch (Exception ex)
            {
                return new UserInformation();
            }
        }
    }
}
