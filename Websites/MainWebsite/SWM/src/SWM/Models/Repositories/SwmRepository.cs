﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.JsonModels;
using Microsoft.AspNetCore.Identity;
using SWM.Models.ApiModels;
using SWM.ViewModels;

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
                var location = _ctx.UserLocations.FirstOrDefault(l => l.Name == locationName);
                if (user != null && location != null)
                {
                    ProductsToUser[] ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                    foreach (var cu in ctou)
                    {
                        productInfos.Add(new ProductInfo()
                        {
                            ProductName = GetProductInformation(cu.ProductID).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.CropToUserId == cu.Id && 
                            cd.UserLocationToMachineId == _ctx.UserLocationToMachines.FirstOrDefault(ul => ul.UserLocationId == location.Id).Id).Select(cd => cd.Weight).Sum()
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

        public async Task<bool> CheckUserPassword(SwmUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public bool AddNewDataFromMachine(DataFromMachineModel data, string userId, int machineId)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId).Result;
                var machine = _ctx.MachineInformations.FirstOrDefault(m => m.Id == machineId);
                var product = _ctx.ProductInformations.FirstOrDefault(p => p.Id == data.ProductId);
                var location = _ctx.UserLocations.FirstOrDefault(l => (l.Id == data.LocationId) && (l.UserId == userId));
                var ptou = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.ProductID == data.ProductId && pu.UserId == userId);

                if (user != null && machine != null && product != null && location != null && ptou != null)
                {
                    var utom = _ctx.UserLocationToMachines.FirstOrDefault(um => (um.MachineId == machineId) && (um.UserLocationId == location.Id));
                    if (utom == null)
                    {
                        _ctx.UserLocationToMachines.Add(new UserLocationToMachine()
                        {
                            MachineId = machineId,
                            UserLocationId = location.Id
                        });
                        _ctx.SaveChanges();
                        utom = _ctx.UserLocationToMachines.FirstOrDefault(um => (um.MachineId == machineId) && (um.UserLocationId == location.Id));
                    }

                    _ctx.CropDatas.Add(new CropData
                    {
                        CropToUserId = ptou.Id,
                        UserLocationToMachineId = utom.Id,
                        DateTime = data.DateAndTime,
                        Weight = data.weight
                    });
                    _ctx.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TableDataModel> GetDataForDataTable(SwmUser user)
        {
            try
            {
                int counter = 1;
                List<TableDataModel> tableData = new List<TableDataModel>();
                List<ProductsToUser> produtToUsers = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                List<CropData> cropDatas = _ctx.CropDatas.Where(cd => produtToUsers.Any(pu => pu.Id == cd.CropToUserId)).ToList();

                foreach (var cropData in cropDatas)
                {
                    tableData.Add(new TableDataModel()
                    {
                        No = counter++,
                        Name = _ctx.ProductInformations.FirstOrDefault(pi => pi.Id == produtToUsers.FirstOrDefault(pu => pu.Id == cropData.CropToUserId).ProductID).Name,
                        Weight = cropData.Weight,
                        DateAndTime = cropData.DateTime,
                        Location = _ctx.UserLocations.FirstOrDefault(ul => ul.Id == _ctx.UserLocationToMachines.FirstOrDefault(um => um.Id == cropData.UserLocationToMachineId).UserLocationId).Address,
                        MachineId = _ctx.UserLocationToMachines.FirstOrDefault(um => um.Id == cropData.UserLocationToMachineId).MachineId
                    });
                }

                return tableData;
            }
            catch (Exception ex)
            {
                return new List<TableDataModel>();
            }
        }

        public async Task<SwmUser> GetUserByUserId(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
    }
}
