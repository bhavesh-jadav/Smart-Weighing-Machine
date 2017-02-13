using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private RoleManager<UserRoleManager> _roleManager;

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
        public string GetStateName(int stateId)
        {
            return _ctx.States.FirstOrDefault(s => s.Id == stateId).Name;
        }
        public List<LocationInfoModel> GetLocationInfoByUserName(string userName)
        {
            try
            {
                List<LocationInfoModel> locationInfos = new List<LocationInfoModel>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    UserLocation[] farmLocations = _ctx.UserLocations.Where(f => f.UserId == user.Id).ToArray();
                    foreach (var floc in farmLocations)
                    {
                        locationInfos.Add(new LocationInfoModel()
                        {
                            Name = floc.Name,
                            Address = floc.Address + ", " + GetStateName(floc.StateId) + ", " + GetCountryName(floc.CountryId) + ", " +
                                      "Pin Number: " + floc.PinNo.ToString(),
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.UserLocationToMachineId == _ctx.UserLocationToMachines.FirstOrDefault(um => um.UserLocationId == floc.Id).Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return locationInfos;
                }
                else
                    return new List<LocationInfoModel>();

            }
            catch (Exception ex)
            {
                return new List<LocationInfoModel>();
            }
        }
        public List<ProductInfoModel> GetProductInfoByUserName(string userName)
        {
            try
            {
                List<ProductInfoModel> productInfo = new List<ProductInfoModel>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    ProductsToUser[] ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                    foreach(var cu in ctou)
                    {
                        productInfo.Add(new ProductInfoModel()
                        {
                            ProductName = GetProductInformation(cu.ProductID).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.CropToUserId == cu.Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return productInfo;
                }
                else
                {
                    return new List<ProductInfoModel>();
                }
            }
            catch (Exception ex)
            {
                return new List<ProductInfoModel>();
            }
        }
        public async Task<SwmUser> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public List<ProductInfoModel> GetProductInfoByUserNameAndLocation(string locationName, string userName)
        {
            try
            {
                List<ProductInfoModel> productInfos = new List<ProductInfoModel>();
                var user = GetUserByUserName(userName).Result;
                var location = _ctx.UserLocations.FirstOrDefault(l => l.Name == locationName);
                if (user != null && location != null)
                {
                    ProductsToUser[] ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToArray();
                    foreach (var cu in ctou)
                    {
                        productInfos.Add(new ProductInfoModel()
                        {
                            ProductName = GetProductInformation(cu.ProductID).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.CropToUserId == cu.Id && 
                            cd.UserLocationToMachineId == _ctx.UserLocationToMachines.FirstOrDefault(ul => ul.UserLocationId == location.Id).Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return productInfos;
                }
                else
                    return new List<ProductInfoModel>();
            }
            catch (Exception ex)
            {
                return new List<ProductInfoModel>();
            }
        }
        public ProductInformation GetProductInformation(int productId)
        {
            return _ctx.ProductInformations.FirstOrDefault(p => p.Id == productId);
        }
        public UserInformationModel GetUserInformationForAPI(string userName)
        {
            try
            {
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    UserInformationModel userInformation = new UserInformationModel();
                    userInformation.TotalProducts = _ctx.ProductsToUsers.Where(p => p.UserId == user.Id).Count();

                    List<ProductInfoModel> productInfo = GetProductInfoByUserName(userName);
                    foreach (var pinfo in productInfo)
                        userInformation.TotalWeight += pinfo.TotalWeight;

                    userInformation.SubTypeName = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Id == _ctx.UserToSubscriptions.FirstOrDefault(u => u.UserID == user.Id).SubscriptionTypeId).Name;

                    return userInformation;
                }
                else
                    return new UserInformationModel();
            }
            catch (Exception ex)
            {
                return new UserInformationModel();
            }
        }
        public async Task<bool> CheckUserPassword(SwmUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public bool AddNewDataFromMachine(DataFromMachineModel data)
        {
            try
            {
                var user = _userManager.FindByIdAsync(data.UserId).Result;
                var machine = _ctx.MachineInformations.FirstOrDefault(m => m.Id == data.MachineId);
                var product = _ctx.ProductInformations.FirstOrDefault(p => p.Id == data.ProductId);
                var location = _ctx.UserLocations.FirstOrDefault(l => (l.Id == data.LocationId) && (l.UserId == data.UserId));
                var ptou = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.ProductID == data.ProductId && pu.UserId == data.UserId);

                if (user != null && machine != null && product != null && location != null && ptou != null)
                {
                    var utom = _ctx.UserLocationToMachines.FirstOrDefault(um => (um.MachineId == data.MachineId) && (um.UserLocationId == location.Id));
                    if (utom == null)
                    {
                        _ctx.UserLocationToMachines.Add(new UserLocationToMachine()
                        {
                            MachineId = data.MachineId,
                            UserLocationId = location.Id
                        });
                        var mu = _ctx.MachineToUsers.FirstOrDefault(m => m.UserID == user.Id);
                        if (mu == null)
                            _ctx.MachineToUsers.Add(new MachineToUser() { MachineId = data.MachineId, UserID = user.Id });
                        _ctx.SaveChanges();
                        utom = _ctx.UserLocationToMachines.FirstOrDefault(um => (um.MachineId == data.MachineId) && (um.UserLocationId == location.Id));
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
                List<CropData> cropDatas = _ctx.CropDatas.Where(cd => produtToUsers.Any(pu => pu.Id == cd.CropToUserId)).OrderByDescending(cd => cd.DateTime).ToList();
                List<UserLocationToMachine> userLocationToMachine = _ctx.UserLocationToMachines.Where(ul => cropDatas.Any(cd => cd.UserLocationToMachineId == ul.Id)).ToList();
                List<UserLocation> userLocations = _ctx.UserLocations.Where(ul => userLocationToMachine.Any(um => um.UserLocationId == ul.Id)).ToList();

                //key productToUserId value productName
                Dictionary<int, string> productInformation = new Dictionary<int, string>();
                foreach (var data in produtToUsers)
                    productInformation.Add(data.Id, _ctx.ProductInformations.FirstOrDefault(pi => pi.Id == data.ProductID).Name);

                //Key locationId value locationname
                Dictionary<int, string> locationInfo = new Dictionary<int, string>();
                foreach (var data in userLocationToMachine)
                    locationInfo.Add(data.UserLocationId, _ctx.UserLocations.FirstOrDefault(ul => ul.Id == data.UserLocationId).Name);

                foreach (var cropData in cropDatas)
                {
                    tableData.Add(new TableDataModel()
                    {
                        No = counter++,
                        Name = productInformation[cropData.CropToUserId],
                        Weight = cropData.Weight,
                        DateAndTime = cropData.DateTime,
                        Location = locationInfo[userLocationToMachine.FirstOrDefault(um => um.Id == cropData.UserLocationToMachineId).UserLocationId],
                        MachineId = userLocationToMachine.FirstOrDefault(um => um.Id == cropData.UserLocationToMachineId).MachineId
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
                var password = CreatePassword(10);
                var userName = userModel.FullName.Replace(" ", String.Empty).ToLower() + userModel.SubscriptionTypes[0].Trim().ToLower() + password;
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
                var subscriptionCount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "SubscriptionCount");
                var subscriptionId = Int32.Parse(subscriptionCount.Value);
                subscriptionId++;
                _ctx.UserToSubscriptions.Add(new UserToSubscription() { UserID = user.Id, SubscriptionTypeId = subscriptionTypeId, SubscriptionId = subscriptionId });
                subscriptionCount.Value = subscriptionId.ToString();
                _ctx.SaveChanges();

                userName = userModel.SubscriptionTypes[0].Trim().ToLower() + subscriptionId.ToString();
                user.UserName = userName;
                await _userManager.UpdateAsync(user);

                string body = String.Format(System.IO.File.ReadAllText("MailBodies/_mailBodies/Registration.html"), userName, password);
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
        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
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
                var user = await _userManager.FindByIdAsync(userModel.UserId.Trim());
                if (user != null && user.UserName == userModel.UserName.Trim())
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
        public bool AddNewLocation(AddNewLocationModel newLocation, string userId)
        {
            try
            {
                var state = _ctx.States.FirstOrDefault(s => s.Name.ToLower() == newLocation.State.ToLower().Trim());
                var country = _ctx.Countries.FirstOrDefault(s => s.Name.ToLower() == newLocation.Country.ToLower().Trim());

                if (state == null)
                {
                    _ctx.States.Add(new State() { Name = newLocation.State });
                    _ctx.SaveChanges();
                    state = _ctx.States.FirstOrDefault(s => s.Name.ToLower() == newLocation.State.ToLower().Trim());
                }
                if (country == null)
                {
                    _ctx.Countries.Add(new Country() { Name = newLocation.Country });
                    _ctx.SaveChanges();
                    country = _ctx.Countries.FirstOrDefault(s => s.Name.ToLower() == newLocation.Country.ToLower().Trim());
                }
                _ctx.UserLocations.Add(new UserLocation() {
                    Name = newLocation.Name,
                    Address = newLocation.Address,
                    StateId = state.Id,
                    CountryId = country.Id,
                    PinNo = Int32.Parse(newLocation.PinNo),
                    UserId = userId
                });
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public UserDashboardModel GetDashBoardForUser(string userId)
        {
            var userDashboard = new UserDashboardModel();
            Dictionary<int, string> countries = _ctx.Countries.ToDictionary(c => c.Id, c => c.Name);
            Dictionary<int, string> states = _ctx.States.ToDictionary(c => c.Id, c => c.Name);
            Dictionary<int, string> productsInfo = _ctx.ProductInformations.ToDictionary(p => p.Id, p => p.Name);
            try
            {
                if (_ctx.ProductsToUsers.FirstOrDefault(pu => pu.UserId == userId) != null && _ctx.UserLocations.FirstOrDefault(u => u.UserId == userId) != null)
                {
                    List<UserLocation> userLocations = _ctx.UserLocations.Where(l => l.UserId == userId).ToList();
                    foreach (var userLocation in userLocations)
                    {
                        userDashboard.UserLocations.Add(new AddNewLocationModel()
                        {
                            Name = userLocation.Name,
                            Address = userLocation.Address,
                            Country = countries[userLocation.CountryId],
                            State = states[userLocation.StateId],
                            PinNo = userLocation.PinNo.ToString()
                        });
                    }
                    var ctou = _ctx.ProductsToUsers.Where(cu => cu.UserId == userId).ToArray();
                    var cropDatas = _ctx.CropDatas.Where(cd => ctou.Any(c => cd.CropToUserId == c.Id));
                    userDashboard.TotalWeight = cropDatas.Select(cd => cd.Weight).Sum();
                    userDashboard.TotalProducts = _ctx.ProductsToUsers.Where(pu => pu.UserId == userId).ToArray().Length;
                    userDashboard.TotalLocation = _ctx.UserLocations.Where(ul => ul.UserId == userId).ToArray().Length;

                    foreach (var cu in ctou)
                    {
                        var product = productsInfo[cu.ProductID];
                        userDashboard.ProductsIntoAccount += product;
                        userDashboard.ProductsIntoAccount += ", ";
                    }
                    userDashboard.ProductsIntoAccount = userDashboard.ProductsIntoAccount.Substring(0, userDashboard.ProductsIntoAccount.Length - 2);
                    userDashboard.UserName = _userManager.FindByIdAsync(userId).Result.UserName;
                    if(cropDatas.ToArray().Length > 0)
                    {
                        var cropToUserId = cropDatas.OrderByDescending(cd => cd.DateTime).ToArray()[0].CropToUserId;
                        var productId = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.Id == cropToUserId).ProductID;
                        userDashboard.LastUpdatedProduct = _ctx.ProductInformations.FirstOrDefault(pi => pi.Id == productId).Name;
                    }
                    else
                        userDashboard.HaveSomeData = false;
                    return userDashboard;
                }
                else
                {
                    userDashboard.IsNewUser = true;
                    return userDashboard;
                }
            }
            catch (Exception ex)
            {
                return userDashboard;
            }
        }
        public bool AddNewProduct(string userId, AddNewProductModel newProduct)
        {
            try
            {
                var product = _ctx.ProductInformations.FirstOrDefault(pi => pi.Name == newProduct.Name);
                if (product == null)
                {
                    _ctx.Add(new ProductInformation() { Name = newProduct.Name });
                    _ctx.SaveChanges();
                    product = _ctx.ProductInformations.FirstOrDefault(pi => pi.Name == newProduct.Name);
                }
                var productToUser = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.UserId == userId && pu.ProductID == product.Id);
                if(productToUser == null)
                {
                    _ctx.Add(new ProductsToUser() { UserId = userId, ProductID = product.Id });
                    _ctx.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public AdminDashboardModel GetDashBoardForAdmin()
        {
            AdminDashboardModel adminDashboard = new AdminDashboardModel();
            try
            {
                adminDashboard.TotalLocations = _ctx.UserLocations.Count();
                adminDashboard.TotalPorducts = _ctx.ProductsToUsers.Count();
                var users = _ctx.SwmUsers.ToList();
                foreach (var user in users)
                {
                    if (_userManager.IsInRoleAsync(user, "user").Result)
                        adminDashboard.TotalUsers++;
                }
                adminDashboard.TotalWeight = _ctx.CropDatas.Select(cd => cd.Weight).Sum();

                return adminDashboard;
            }
            catch (Exception ex)
            {
                return adminDashboard;
            }
        }
        public List<KeyValuePair<int, string>> GetProductNames(string userId)
        {
            var ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == userId);
            var pr = _ctx.ProductInformations.Where(pi => ptou.Any(pu => pu.ProductID == pi.Id)).ToList();
            List<KeyValuePair<int, string>> produts = new List<KeyValuePair<int, string>>();
            foreach (var product in pr)
                produts.Add(new KeyValuePair<int, string>(product.Id, product.Name));
            return produts;
        }
        public List<KeyValuePair<int, string>> GetLocationNames(string userId)
        {
            var lc = _ctx.UserLocations.Where(ul => ul.UserId == userId).ToList();
            List<KeyValuePair<int, string>> locations = new List<KeyValuePair<int, string>>();
            foreach (var location in lc)
                locations.Add(new KeyValuePair<int, string>(location.Id, location.Name));
            return locations;
        }
        private string getProductNameFromProductToUserId(int ptouId)
        {
            return _ctx.ProductInformations.FirstOrDefault(pi => pi.Id == _ctx.ProductsToUsers.FirstOrDefault(pu => pu.Id == ptouId).ProductID).Name;
        }
        private bool IsBetween(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }
        public List<ProductDataMonthWiseModel> GetProductDataMonthWise(string userName, int startMonth, int startYear, int endMonth, int endYear)
        {
            List<ProductDataMonthWiseModel> monthWiseData = new List<ProductDataMonthWiseModel>();
            try
            {
                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                SwmUser user = _userManager.FindByNameAsync(userName).Result;
                List<ProductsToUser> ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                List<CropData> cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.CropToUserId) && IsBetween(cd.DateTime,startDate, endDate)).ToList();
                foreach (var cropData in cropDatas)
                {
                    if (monthWiseData.Any(md => md.Date.Month == cropData.DateTime.Month && md.Date.Year == cropData.DateTime.Year))
                    {
                        var d = monthWiseData.Find(md => md.Date.Month == cropData.DateTime.Month && md.Date.Year == cropData.DateTime.Year);
                        string productName = getProductNameFromProductToUserId(cropData.CropToUserId);
                        if (d.ProductInformation.Any(pi => pi.ProductName == productName))
                            d.ProductInformation.Find(pi => pi.ProductName == productName).TotalWeight += cropData.Weight;
                        else
                            d.ProductInformation.Add(new ProductInfoModel() { ProductName = productName, TotalWeight = cropData.Weight });
                    }
                    else
                        monthWiseData.Add(new ProductDataMonthWiseModel() { Date = new DateTime(cropData.DateTime.Year, cropData.DateTime.Month, 1), ProductInformation = new List<ProductInfoModel>() { new ProductInfoModel() { ProductName = getProductNameFromProductToUserId(cropData.CropToUserId), TotalWeight = cropData.Weight}}});
                }

                return monthWiseData.OrderBy(md => md.Date).ToList();
            }
            catch (Exception ex)
            {
                return monthWiseData;
            }
        }
        public List<DateTime> GetUserMonths(string userName)
        {
            List<DateTime> userMonths = new List<DateTime>();
            try
            {
                var user = _userManager.FindByNameAsync(userName).Result;
                var ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                var cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.CropToUserId)).OrderBy(cd => cd.DateTime).GroupBy(cd => new { cd.DateTime.Month, cd.DateTime.Year }).ToList();
                foreach (var cropData in cropDatas)
                    userMonths.Add(new DateTime(cropData.Key.Year, cropData.Key.Month, 1));
                return userMonths;
            }
            catch (Exception)
            {
                return userMonths;
            }
        }
    }
}
