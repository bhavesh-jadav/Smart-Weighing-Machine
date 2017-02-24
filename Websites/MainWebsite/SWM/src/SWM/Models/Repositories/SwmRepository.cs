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
using System.Collections;

namespace SWM.Models.Repositories
{
    public class SwmRepository : ISwmRepository
    {
        private SwmContext _ctx;
        private UserManager<SwmUser> _userManager;
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private RoleManager<UserRoleManager> _roleManager;

        public SwmRepository(
            SwmContext ctx, UserManager<SwmUser> userManager, 
            IMailService mailService, IConfigurationRoot config,
            RoleManager<UserRoleManager> roleManager)
        {
            _ctx = ctx;
            _userManager = userManager;
            _mailService = mailService;
            _config = config;
            _roleManager = roleManager;
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
                Dictionary<int, string> productsInformation = _ctx.ProductInformations.ToDictionary(p => p.Id, p => p.Name);
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    List<ProductsToUser> ptou = _ctx.ProductsToUsers.Where(cu => cu.UserId == user.Id).ToList();
                    List<CropData> cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.ProductToUserId)).ToList();
                    foreach (var pu in ptou)
                    {
                        productInfo.Add(new ProductInfoModel()
                        {
                            ProductName = productsInformation[pu.ProductId],
                            TotalWeight = cropDatas.Where(cd => cd.ProductToUserId == pu.Id).Select(cd => cd.Weight).Sum()
                        });
                    }
                    return productInfo;
                }
                else
                {
                    List<ProductsToUser> ptou = _ctx.ProductsToUsers.ToList();
                    List<CropData> cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.ProductToUserId)).ToList();
                    foreach (var pu in ptou)
                    {
                        var productName = productsInformation[pu.ProductId];
                        var data = productInfo.FirstOrDefault(d => d.ProductName == productName);
                        if (data != null)
                            data.TotalWeight += cropDatas.Where(cd => cd.ProductToUserId == pu.Id).Select(cd => cd.Weight).Sum();
                        else
                        {
                            productInfo.Add(new ProductInfoModel()
                            {
                                ProductName = productName,
                                TotalWeight = cropDatas.Where(cd => cd.ProductToUserId == pu.Id).Select(cd => cd.Weight).Sum()
                            });
                        }
                    }
                    return productInfo;
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
                            ProductName = GetProductInformation(cu.ProductId).Name,
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.ProductToUserId == cu.Id &&
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
        public async Task<bool> CheckUserPassword(SwmUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public bool AddNewDataFromMachine(DataFromMachineModel data)
        {
            try
            {
                var user = _userManager.FindByIdAsync(data.UserId).Result;
                var machine = _ctx.MachineInformations.FirstOrDefault(m => m.MachineId == data.MachineId);
                var product = _ctx.ProductInformations.FirstOrDefault(p => p.Id == data.ProductId);
                var location = _ctx.UserLocations.FirstOrDefault(l => (l.Id == data.LocationId) && (l.UserId == data.UserId));
                var ptou = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.ProductId == data.ProductId && pu.UserId == data.UserId);

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
                        ProductToUserId = ptou.Id,
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
                List<CropData> cropDatas = _ctx.CropDatas.Where(cd => produtToUsers.Any(pu => pu.Id == cd.ProductToUserId)).OrderByDescending(cd => cd.DateTime).ToList();
                List<UserLocationToMachine> userLocationToMachine = _ctx.UserLocationToMachines.Where(ul => cropDatas.Any(cd => cd.UserLocationToMachineId == ul.Id)).ToList();
                List<UserLocation> userLocations = _ctx.UserLocations.Where(ul => userLocationToMachine.Any(um => um.UserLocationId == ul.Id)).ToList();

                //key productToUserId value productName
                Dictionary<int, string> productInformation = new Dictionary<int, string>();
                foreach (var data in produtToUsers)
                    productInformation.Add(data.Id, _ctx.ProductInformations.FirstOrDefault(pi => pi.Id == data.ProductId).Name);

                //Key locationId value locationname
                Dictionary<int, string> locationInfo = new Dictionary<int, string>();
                foreach (var data in userLocationToMachine)
                    locationInfo.Add(data.UserLocationId, _ctx.UserLocations.FirstOrDefault(ul => ul.Id == data.UserLocationId).Address);

                foreach (var cropData in cropDatas)
                {
                    tableData.Add(new TableDataModel()
                    {
                        No = counter++,
                        Name = productInformation[cropData.ProductToUserId],
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
                    CountryId = country.Id,
                    RegisterDate = DateTime.Now
                }, password);
                var user = await _userManager.FindByNameAsync(userName);
                await _userManager.AddToRoleAsync(user, "user");

                var subscriptionTypeId = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Name.ToLower() == userModel.SubscriptionTypes[0].ToLower()).Id;
                var UserCounts = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "UserCounts");
                int userCounts = Int32.Parse(UserCounts.Value);
                userCounts++;

                _ctx.UserToSubscriptions.Add(new UserToSubscription() { UserID = user.Id, SubscriptionTypeId = subscriptionTypeId, SubscriptionId = Guid.NewGuid().ToString().Replace("-", "") });
                UserCounts.Value = userCounts.ToString();
                _ctx.SaveChanges();

                userName = userModel.SubscriptionTypes[0].Trim().ToLower() + userCounts.ToString();
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
                foreach (var user in users)
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
                            Address = user.Address + ", Pin: " + user.PinNo + ", State: " + _ctx.States.FirstOrDefault(s => s.Id == user.StateId).Name + ", Country: " + _ctx.Countries.FirstOrDefault(c => c.Id == user.CountryId).Name,
                            DateRegisterd = user.RegisterDate
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
                        var cds = _ctx.CropDatas.Where(cd => cd.ProductToUserId == row.Id).ToList();
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
                _ctx.UserLocations.Add(new UserLocation()
                {
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
            var user = _userManager.FindByIdAsync(userId).Result;
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
                    var cropDatas = _ctx.CropDatas.Where(cd => ctou.Any(c => cd.ProductToUserId == c.Id));
                    userDashboard.TotalWeight = cropDatas.Select(cd => cd.Weight).Sum();
                    userDashboard.TotalProducts = _ctx.ProductsToUsers.Where(pu => pu.UserId == userId).ToArray().Length;
                    userDashboard.TotalLocation = _ctx.UserLocations.Where(ul => ul.UserId == userId).ToArray().Length;
                    userDashboard.FullName = user.FullName;
                    foreach (var cu in ctou)
                    {
                        var product = productsInfo[cu.ProductId];
                        userDashboard.ProductsIntoAccount += product;
                        userDashboard.ProductsIntoAccount += ", ";
                    }
                    userDashboard.ProductsIntoAccount = userDashboard.ProductsIntoAccount.Substring(0, userDashboard.ProductsIntoAccount.Length - 2);
                    userDashboard.UserName = user.UserName;
                    if (cropDatas.ToArray().Length > 0)
                    {
                        var cropToUserId = cropDatas.OrderByDescending(cd => cd.DateTime).ToArray()[0].ProductToUserId;
                        var productId = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.Id == cropToUserId).ProductId;
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
                var productToUser = _ctx.ProductsToUsers.FirstOrDefault(pu => pu.UserId == userId && pu.ProductId == product.Id);
                if (productToUser == null)
                {
                    _ctx.Add(new ProductsToUser() { UserId = userId, ProductId = product.Id });
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
            var pr = _ctx.ProductInformations.Where(pi => ptou.Any(pu => pu.ProductId == pi.Id)).ToList();
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
        private bool IsBetween(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }
        public List<ProductDataMonthWiseModel> GetProductDataMonthWise(string userName, int startMonth, int startYear, int endMonth, int endYear)
        {
            List<ProductDataMonthWiseModel> monthWiseData = new List<ProductDataMonthWiseModel>();
            try
            {
                List<CropData> cropDatas;
                Dictionary<int, int> ptous;
                Dictionary<int, string> products = _ctx.ProductInformations.ToDictionary(pi => pi.Id, pi => pi.Name);
                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                if (userName != "")
                {
                    SwmUser user = _userManager.FindByNameAsync(userName).Result;
                    ptous = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToDictionary(pu => pu.Id, pu => pu.ProductId);
                    cropDatas = _ctx.CropDatas.Where(cd => ptous.ContainsKey(cd.ProductToUserId) && IsBetween(cd.DateTime, startDate, endDate)).ToList();
                }
                else
                {
                    cropDatas = _ctx.CropDatas.Where(cd => IsBetween(cd.DateTime, startDate, endDate)).ToList();
                    ptous = _ctx.ProductsToUsers.ToDictionary(pu => pu.Id, pu => pu.ProductId);
                }

                foreach (var cropData in cropDatas)
                {
                    var data = monthWiseData.FirstOrDefault(md => md.Date.Month == cropData.DateTime.Month && md.Date.Year == cropData.DateTime.Year);
                    string productName = products[ptous[cropData.ProductToUserId]];
                    if (data != null)
                    {
                        var pinfo = data.ProductInformation.FirstOrDefault(pi => pi.ProductName == productName);
                        if (pinfo != null)
                            pinfo.TotalWeight += cropData.Weight;
                        else
                            data.ProductInformation.Add(new ProductInfoModel() { ProductName = productName, TotalWeight = cropData.Weight });
                    }
                    else
                        monthWiseData.Add(new ProductDataMonthWiseModel() { Date = new DateTime(cropData.DateTime.Year, cropData.DateTime.Month, 1), ProductInformation = new List<ProductInfoModel>() { new ProductInfoModel() { ProductName = productName, TotalWeight = cropData.Weight } } });
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
                if (userName != "")
                {
                    var user = _userManager.FindByNameAsync(userName).Result;
                    var ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                    var cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.ProductToUserId)).OrderBy(cd => cd.DateTime).GroupBy(cd => new { cd.DateTime.Month, cd.DateTime.Year }).ToList();
                    foreach (var cropData in cropDatas)
                        userMonths.Add(new DateTime(cropData.Key.Year, cropData.Key.Month, 1));
                    return userMonths;
                }
                else
                {
                    var cropDatas = _ctx.CropDatas.GroupBy(cd => new { cd.DateTime.Month, cd.DateTime.Year }).ToList().OrderBy(cd => cd.Key.Year).ThenBy(cd => cd.Key.Month);
                    foreach (var cropData in cropDatas)
                        userMonths.Add(new DateTime(cropData.Key.Year, cropData.Key.Month, 1));
                    return userMonths;
                }
            }
            catch (Exception)
            {
                return userMonths;
            }
        }
        public PublicDashboardModel GetDashBoardForPublic()
        {
            PublicDashboardModel publicDashboard = new PublicDashboardModel();
            try
            {
                publicDashboard.TotalUsers = _ctx.SwmUsers.ToList().Where(u => _userManager.IsInRoleAsync(u, "user").Result).Count();
                publicDashboard.TotalWeight = _ctx.CropDatas.Select(cd => cd.Weight).Sum();
                publicDashboard.TotalMachines = _ctx.MachineInformations.Count();
                publicDashboard.TotalProducts = _ctx.ProductInformations.Count();
                publicDashboard.TotalUserLocations = _ctx.UserLocations.Count();
                publicDashboard.LastUserRegisterd = _ctx.SwmUsers.OrderByDescending(u => u.RegisterDate).ToList().Where(u => _userManager.IsInRoleAsync(u, "user").Result).First().FullName;

                return publicDashboard;
            }
            catch (Exception ex)
            {
                return publicDashboard;
            }
        }
        public List<SearchUserModel> GetAllUsers(int pageNo)
        {
            List<SearchUserModel> result = new List<SearchUserModel>();
            try
            {
                if (pageNo < 1)
                    pageNo = 1;
                int numberOfUsersPerRequest = 50;
                int counter = ((numberOfUsersPerRequest * pageNo) - 50) + 1;
                var userRole = _roleManager.FindByNameAsync("user").Result;
                var users = _ctx.SwmUsers
                    .Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id))
                    .Skip(pageNo == 1 ? 0 : (pageNo*numberOfUsersPerRequest) - 50)
                    .Take(numberOfUsersPerRequest)
                    .Select(u => new
                    {
                        Id = u.Id,
                        fullName = u.FullName,
                        state = u.State.Name,
                        country = u.Country.Name,
                        subId = u.UserToSubscription.SubscriptionId
                    }).ToList();

                var productsToUsers = _ctx.ProductsToUsers
                        .Select(pu => new { userId = pu.UserId, productName = pu.ProductInformation.Name });

                foreach (var user in users)
                {
                    string products = "";
                    var productData = productsToUsers.Where(pu => pu.userId == user.Id).ToList();
                    foreach (var data in productData)
                    {
                        products += data.productName;
                        products += " ";
                    }

                    products = products.TrimEnd();
                    products = products.Replace(" ", ", ");
                    result.Add(new SearchUserModel()
                    {
                        No = counter++,
                        FullName = user.fullName,
                        Country = user.country,
                        State = user.state,
                        ProductsIntoAccount = products,
                        SubId = user.subId
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public List<SearchUserModel> GetSearchResultForUserByFullName(string fullName)
        {
            List<SearchUserModel> result = new List<SearchUserModel>();
            try
            {
                int counter = 1;
                var userRole = _roleManager.FindByNameAsync("user").Result;
                var users = _ctx.SwmUsers
                    .Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id))
                    .Where(u => u.FullName.ToLower().Contains(fullName.Trim().ToLower()) || u.FullName.ToLower() == fullName.Trim().ToLower())
                    .Select(u => new
                    {
                        Id = u.Id,
                        fullName = u.FullName,
                        state = u.State.Name,
                        country = u.Country.Name,
                        subId = u.UserToSubscription.SubscriptionId
                    }).ToList();

                var productsToUsers = _ctx.ProductsToUsers
                        .Select(pu => new { userId = pu.UserId, productName = pu.ProductInformation.Name });

                foreach (var user in users)
                {
                    string products = "";
                    var productData = productsToUsers.Where(pu => pu.userId == user.Id).ToList();
                    foreach (var data in productData)
                    {
                        products += data.productName;
                        products += " ";
                    }

                    products = products.TrimEnd();
                    products = products.Replace(" ", ", ");
                    result.Add(new SearchUserModel()
                    {
                        No = counter++,
                        FullName = user.fullName,
                        Country = user.country,
                        State = user.state,
                        ProductsIntoAccount = products,
                        SubId = user.subId
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public List<SearchUserModel> AdvanceSearchResults(AdvanceSearchModel parameters)
        {
            List<SearchUserModel> result = new List<SearchUserModel>();
            try
            {
                int satisfyProducts, satisfyLocation, satisfyStates, satisfyCountries;

                Dictionary<int, string> Countries = _ctx.Countries.ToDictionary(c => c.Id, c => c.Name);
                Dictionary<int, string> States = _ctx.States.ToDictionary(c => c.Id, c => c.Name);
                Dictionary<int, string> ProductsInfo = _ctx.ProductInformations.ToDictionary(p => p.Id, p => p.Name);

                string[] names, products, states, countries;
                names = products = states = countries = null;
                if (!string.IsNullOrEmpty(parameters.FullNames))
                    names = parameters.FullNames.Split(',');
                if (!string.IsNullOrEmpty(parameters.Products))
                    products = parameters.Products.Split(',');
                if (!string.IsNullOrEmpty(parameters.States))
                    states = parameters.States.Split(',');
                if (!string.IsNullOrEmpty(parameters.Countries))
                    countries = parameters.Countries.Split(',');

                List<SwmUser> users = new List<SwmUser>();
                if (names != null)
                {
                    foreach (var name in names)
                        users.AddRange(_ctx.SwmUsers.Where(u => u.FullName.Contains(name) || u.FullName == name).ToList()
                            .Where(u => _userManager.IsInRoleAsync(u, "user").Result).ToList());
                }
                else
                    users = _ctx.SwmUsers.ToList().Where(u => _userManager.IsInRoleAsync(u, "user").Result).ToList();

                foreach (var user in users.ToList())
                {
                    satisfyProducts = satisfyLocation = satisfyStates = satisfyCountries = 0;
                    List<UserLocation> userLocations = _ctx.UserLocations.Where(ul => ul.UserId == user.Id).ToList();
                    List<ProductsToUser> ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                    if (products != null)
                    {
                        foreach (var product in products)
                        {
                            var productId = ProductsInfo.FirstOrDefault(pu => pu.Value.ToLower().Trim() == product.ToLower().Trim()).Key;
                            if (ptou.Any(pu => pu.ProductId == productId))
                            {
                                satisfyProducts = 1;
                                break;
                            }
                        }
                        if (satisfyProducts == 0)
                            users.Remove(user);
                    }
                    if (parameters.Location != null)
                    {
                        foreach (var locaions in userLocations)
                        {
                            if (locaions.Address.ToLower().Contains(parameters.Location.Trim().ToLower()))
                            {
                                satisfyLocation = 1;
                                break;
                            }
                        }
                        if (satisfyLocation == 0)
                            users.Remove(user);
                    }
                    if (states != null)
                    {
                        foreach (var state in states)
                        {
                            foreach (var locations in userLocations)
                            {
                                if (locations.StateId == States.FirstOrDefault(s => s.Value.ToLower().Trim() == state.ToLower().Trim()).Key)
                                {
                                    satisfyStates = 1;
                                    break;
                                }
                            }
                            if (satisfyStates == 0)
                                users.Remove(user);
                        }
                    }
                    if (countries != null)
                    {
                        foreach (var country in countries)
                        {
                            foreach (var locations in userLocations)
                            {
                                if (locations.CountryId == Countries.FirstOrDefault(s => s.Value.ToLower().Trim() == country.ToLower().Trim()).Key)
                                {
                                    satisfyCountries = 1;
                                    break;
                                }
                            }
                            if (satisfyCountries == 0)
                                users.Remove(user);
                        }
                    }
                }

                int counter = 1;
                foreach (var user in users)
                {
                    string productNames = "";
                    Dictionary<int, int> ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToDictionary(pu => pu.Id, pu => pu.ProductId);
                    foreach (var product in ptou.Values)
                    {
                        productNames += ProductsInfo[product];
                        productNames += " ";
                    }
                    productNames = productNames.TrimEnd();
                    productNames = productNames.Replace(" ", ", ");
                    result.Add(new SearchUserModel()
                    {
                        No = counter++,
                        FullName = user.FullName,
                        Country = Countries[user.CountryId],
                        State = States[user.StateId],
                        ProductsIntoAccount = productNames,
                        SubId = _ctx.UserToSubscriptions.FirstOrDefault(us => us.UserID == user.Id).SubscriptionId
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public UserDetailsModel GetUserDetails(string subId)
        {
            UserDetailsModel userDetails = new UserDetailsModel();
            try
            {
                Dictionary<int, string> countries = _ctx.Countries.ToDictionary(c => c.Id, c => c.Name);
                Dictionary<int, string> states = _ctx.States.ToDictionary(c => c.Id, c => c.Name);
                Dictionary<int, string> productsInfo = _ctx.ProductInformations.ToDictionary(p => p.Id, p => p.Name);
                SwmUser user = _ctx.SwmUsers.FirstOrDefault(u => u.Id == _ctx.UserToSubscriptions.FirstOrDefault(us => us.SubscriptionId == subId).UserID);
                List<UserLocation> userLocations = _ctx.UserLocations.Where(ul => ul.UserId == user.Id).ToList();
                Dictionary<int, int> userLocationToMachine = _ctx.UserLocationToMachines.Where(um => userLocations.Count(ul => ul.Id == um.UserLocationId) > 0).
                    ToDictionary(um => um.Id, um => um.UserLocationId);
                List<ProductsToUser> ptou = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToList();
                List<CropData> cropDatas = _ctx.CropDatas.Where(cd => ptou.Any(pu => pu.Id == cd.ProductToUserId)).ToList();
                Dictionary<int, int> productToUsers = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).ToDictionary(pu => pu.Id, pu => pu.ProductId);

                userDetails.SubId = subId;
                userDetails.FullName = user.FullName;
                userDetails.SubType = _ctx.SubscriptionTypes.FirstOrDefault(st => st.Id == _ctx.UserToSubscriptions.FirstOrDefault(us => us.SubscriptionId == subId).SubscriptionTypeId).Name;
                userDetails.TotalWeight = cropDatas.Select(cd => cd.Weight).Sum();
                userDetails.ContactNo = user.PhoneNumber;
                userDetails.Email = user.Email;

                var co = _ctx.CropDatas.Where(c => c.ProductToUserId == 3).ToList();

                List<CropData> sortedCropData;
                int maxDisplayAmount = 15;
                if (cropDatas.Count > maxDisplayAmount)
                    sortedCropData = cropDatas.OrderByDescending(cd => cd.DateTime).Take(maxDisplayAmount).ToList();
                else
                    sortedCropData = cropDatas.OrderByDescending(cd => cd.DateTime).ToList();

                int counter = 1;
                foreach (var cropdata in sortedCropData)
                {
                    userDetails.LatestUpdatedTables.Add(new UserDetailsLatestUpdatedTableModel()
                    {
                        No = counter++,
                        DateAndTime = cropdata.DateTime,
                        Location = userLocations.FirstOrDefault(ul => ul.Id == userLocationToMachine[cropdata.UserLocationToMachineId]).Address,
                        ProductName = productsInfo[productToUsers[cropdata.ProductToUserId]],
                        Weight = cropdata.Weight
                    });
                }

                foreach (var data in productToUsers)
                {
                    userDetails.ProductsIntoAccount.Add(new ProductInformation()
                    {
                        Name = productsInfo[data.Value]
                    });
                }

                foreach (var userLocation in userLocations)
                {
                    userDetails.UserLocations.Add(new AddNewLocationModel()
                    {
                        Name = userLocation.Name,
                        Address = userLocation.Address,
                        Country = countries[userLocation.CountryId],
                        State = states[userLocation.StateId],
                        PinNo = userLocation.PinNo.ToString()
                    });
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                return userDetails;
            }
        }
    }
}
