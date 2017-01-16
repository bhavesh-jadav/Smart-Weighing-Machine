using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWM.JsonModels;
using Microsoft.AspNetCore.Identity;
using SWM.Models.ApiModels;
using SWM.ViewModels;
using SWM.Services;
using Microsoft.Extensions.Configuration;

namespace SWM.Models.Repositories
{
    public class SwmRepository : ISwmRepository
    {
        private SwmContext _ctx;
        private UserManager<SwmUser> _userManager;
        private IMailService _mailService;
        private IConfigurationRoot _config;

        public SwmRepository(SwmContext ctx, UserManager<SwmUser> userManager, IMailService mailService, IConfigurationRoot config)
        {
            _ctx = ctx;
            _userManager = userManager;
            _mailService = mailService;
            _config = config;
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
                        var mu = _ctx.MachineToUsers.FirstOrDefault(m => m.UserID == user.Id);
                        if (mu == null)
                            _ctx.MachineToUsers.Add(new MachineToUser() { MachineId = machineId, UserID = user.Id });
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
        public string[] GetSubscriptionTypes()
        {
            try
            {
                return _ctx.SubscriptionTypes.Select(s => s.Name).ToArray();
            }
            catch (Exception ex)
            {
                return new string[] { };
            }
        }
        public async Task<bool> AddNewUser(AddNewUserModel userModel)
        {
            try
            {
                var userName = userModel.FullName.Replace(" ", String.Empty).ToLower() + userModel.SubscriptionTypes[0].Trim().ToLower();
                var password = "bhavesh123";
                var state = _ctx.States.FirstOrDefault(s => s.Name.ToLower().Trim() == userModel.State.ToLower().Trim());
                if (state == null)
                {
                    _ctx.States.Add(new State { Name = userModel.State });
                    _ctx.SaveChanges();
                    state = _ctx.States.FirstOrDefault(s => s.Name.ToLower().Trim() == userModel.State.ToLower().Trim());
                }
                var country = _ctx.Countries.FirstOrDefault(c => c.Name.ToLower().Trim() == userModel.Country.ToLower().Trim());
                if (country == null)
                {
                    _ctx.Countries.Add(new Country { Name = userModel.Country });
                    _ctx.SaveChanges();
                    country = _ctx.Countries.FirstOrDefault(c => c.Name.ToLower().Trim() == userModel.Country.ToLower().Trim());
                }

                await _userManager.CreateAsync(new SwmUser()
                {
                    Email = userModel.Email,
                    FullName = userModel.FullName,
                    PhoneNumber = userModel.ContactNo,
                    UserName = userName,
                    Address = userModel.Address,
                    PinNo = Int32.Parse(userModel.PinNo),
                    StateId = state.Id,
                    CountryId = country.Id
                }, password);
                var user = await _userManager.FindByNameAsync(userName);
                await _userManager.AddToRoleAsync(user, "user");

                var subscriptionTypeId = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Name.ToLower() == userModel.SubscriptionTypes[0].ToLower()).Id;
                var subscriptionIdCount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "SubscriptionCount");
                var subscriptionId = Int32.Parse(subscriptionIdCount.Value);
                subscriptionId++;
                _ctx.UserToSubscriptions.Add(new UserToSubscription() { UserID = user.Id, SubscriptionTypeId = subscriptionTypeId, SubscriptionId = subscriptionId });
                subscriptionIdCount.Value = subscriptionId.ToString();
                _ctx.SaveChanges();

                string body = String.Format(System.IO.File.ReadAllText("MailBodies/Registration.min.html"), userName, password);
                var res = _mailService.SendMail("SWM", "noreply@swm", userModel.FullName,
                      userModel.Email, "Welcome to SWM", body);

                if (res)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<ShowUserModel> GetAllUsers()
        {
            int counter = 1;
            List<ShowUserModel> allUsers = new List<ShowUserModel>();
            try
            {
                var users = _ctx.SwmUsers.ToList();
                foreach(var user in users)
                {
                    if (_userManager.IsInRoleAsync(user, "user").Result)
                    {
                        allUsers.Add(new ShowUserModel()
                        {
                            No = counter++,
                            FullName = user.FullName,
                            ContactNo = user.PhoneNumber,
                            LogInUserName = user.UserName,
                            UserId = user.Id,
                            SubscriptionType = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Id == _ctx.UserToSubscriptions.FirstOrDefault(us => us.UserID == user.Id).SubscriptionTypeId).Name,
                            Address = user.Address + ", Pin: " + user.PinNo + ", State: " + _ctx.States.FirstOrDefault(s => s.Id == user.StateId).Name + ", Country: " + _ctx.Countries.FirstOrDefault(c => c.Id == user.CountryId).Name
                        });
                    }
                }
                return allUsers;
            }
            catch (Exception ex)
            {
                return allUsers;
            }
        }

        public async Task<bool> RemoveUser(RemoveUserModel userModel)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userModel.UserId);
                if (user != null && user.UserName == userModel.UserName)
                {
                    await _userManager.DeleteAsync(user);
                    var pus = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                    foreach (var row in pus)
                    {
                        var cds = _ctx.CropDatas.Where(cd => cd.CropToUserId == row.Id).ToList();
                        foreach (var cd in cds)
                            _ctx.Remove(cd);
                        _ctx.Remove(row);
                    }
                    var uls = _ctx.UserLocations.Where(ul => ul.UserId == user.Id).ToList();
                    foreach (var ul in uls)
                    {
                        var ulms = _ctx.UserLocationToMachines.Where(ulm => ulm.UserLocationId == ul.Id).ToList();
                        foreach (var ulm in ulms)
                            _ctx.Remove(ulm);
                        _ctx.Remove(ul);
                    }
                    var uss = _ctx.UserToSubscriptions.Where(us => us.UserID == user.Id).ToList();
                    foreach (var us in uss)
                        _ctx.Remove(us);
                    var mus = _ctx.MachineToUsers.Where(mu => mu.UserID == user.Id).ToList();
                    foreach (var mu in mus)
                        _ctx.Remove(mu);
                    await _ctx.SaveChangesAsync();
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
    }
}
