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

        public List<LocationInfoModel> GetLocationInfoByUserName(string userName)
        {
            try
            {
                List<LocationInfoModel> locationInfos = new List<LocationInfoModel>();
                var user = GetUserByUserName(userName).Result;
                if (user != null)
                {
                    var userLocations = _ctx.UserLocations.Where(f => f.UserId == user.Id)
                        .Select(ul => new {id = ul.Id, name = ul.Name, address = ul.Address, state = ul.State.Name, country = ul.Country.Name, pinNo = ul.PinNo}).ToList();
                    foreach (var datum in userLocations)
                    {
                        locationInfos.Add(new LocationInfoModel()
                        {
                            Name = datum.name,
                            Address = datum.address + ", " + datum.state + ", " + datum.country + ", " + "Pin Number: " + datum.pinNo.ToString(),
                            TotalWeight = _ctx.CropDatas.Where(cd => cd.UserLocationToMachine.UserLocation.Id == datum.id).Sum(cd => Convert.ToInt64(cd.Weight))
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
                var user = _userManager.FindByNameAsync(userName).Result;
                if (user != null)
                {
                    productInfo = _ctx.ProductsToUsers
                        .Where(pu => pu.UserId == user.Id)
                        .Select(pu => new ProductInfoModel()
                        {
                            ProductName = pu.ProductInformation.Name,
                            TotalWeight = pu.CropData.Select(cd => Convert.ToInt64(cd.Weight)).Sum()
                        })
                        .ToList();
                }
                return productInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<ProductInfoModel> GetAllProductInformation()
        {
            try
            {
                List<ProductInfoModel> productInfo = new List<ProductInfoModel>();
                productInfo = _ctx.ProductInformations
                    .Select(pi => new ProductInfoModel()
                    {
                        ProductName = pi.Name,
                        TotalWeight = pi.ProductsToUser.SelectMany(pu => pu.CropData).Select(cd => Convert.ToInt64(cd.Weight)).Sum()
                    })
                    .ToList();
                return productInfo;
            }
            catch (Exception ex)
            {
                return null;
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
                var user = _userManager.FindByNameAsync(userName).Result;
                if (user != null)
                {
                    productInfos = _ctx.ProductsToUsers
                        .Where(pu => pu.UserId == user.Id)
                        .Select(pu => new ProductInfoModel()
                        {
                            ProductName = pu.ProductInformation.Name,
                            TotalWeight = pu.CropData
                                    .Where(cd => cd.UserLocationToMachine.UserLocation.Name == locationName)
                                    .Select(cd => Convert.ToInt64(cd.Weight))
                                    .Sum()
                        }).ToList();
                }
                return productInfos;
            }
            catch (Exception ex)
            {
                return null;
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
        public List<TableDataModel> GetUserDataForDataTable(SwmUser user)
        {
            try
            {
                int counter = 1;
                List<TableDataModel> tableData = new List<TableDataModel>();

                tableData = _ctx.CropDatas
                    .Where(cd => cd.ProductsToUser.UserId == user.Id)
                    .OrderByDescending(cd => cd.DateTime)
                    .Select(cd => new TableDataModel()
                    {
                        No = 0,
                        DateAndTime = cd.DateTime,
                        MachineId = cd.UserLocationToMachine.MachineId,
                        ProductName = cd.ProductsToUser.ProductInformation.Name,
                        Weight = cd.Weight,
                        Location = cd.UserLocationToMachine.UserLocation.Address
                    }).ToList();
                foreach (var dataum in tableData)
                    dataum.No = counter++;

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
        public List<ShowUserModel> GetAllUsersForAdmin()
        {
            int counter = 1;
            List<ShowUserModel> allUsers = new List<ShowUserModel>();
            var userRole = _roleManager.FindByNameAsync("user").Result;
            try
            {
                allUsers = _ctx.SwmUsers
                    .Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id))
                    .Select(u => new ShowUserModel()
                    {
                        No = 0,
                        UserId = u.Id,
                        LogInUserName = u.UserName,
                        Address = u.Address,
                        ContactNo = u.PhoneNumber,
                        FullName = u.FullName,
                        DateRegisterd = u.RegisterDate,
                        SubscriptionType = u.UserToSubscription.SubscriptionType.Name
                    }).ToList();

                foreach (var user in allUsers)
                    user.No = counter++;
                
                return allUsers;
            }
            catch (Exception ex)
            {
                return allUsers;
            }
        }
        public bool AddNewLocation(AddNewLocationModel newLocation, string userId)
        {
            try
            {
                var existsLocationName = _ctx.SwmUsers
                    .Where(u => u.Id == userId)
                    .Where(u => u.UserLocation.Any(ul => ul.Name == newLocation.Name))
                    .ToList();

                if (existsLocationName.Count == 0)
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
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool AddNewProduct(string userId, AddNewProductModel newProduct)
        {
            try
            {
                var product = _ctx.ProductInformations.FirstOrDefault(pi => pi.Name.ToLower() == newProduct.Name.Trim().ToLower());
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
                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime endDate = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));
                if (userName == "")
                {
                    for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                    {
                        monthWiseData.Add(new ProductDataMonthWiseModel()
                        {
                            Date = date,
                            ProductInformation = _ctx.ProductInformations
                                .Select(pi => new ProductInfoModel()
                                {
                                    ProductName = pi.Name,
                                    TotalWeight = pi.ProductsToUser.SelectMany(pu => pu.CropData)
                                    .Where(cd => cd.DateTime.Month == date.Month && cd.DateTime.Year == date.Year)
                                    .Select(cd => Convert.ToInt64(cd.Weight)).Sum()
                                })
                                .ToList()
                        });
                    }
                }
                else
                {
                    var user = _userManager.FindByNameAsync(userName).Result;
                    if (user != null)
                    {
                        for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                        {
                            monthWiseData.Add(new ProductDataMonthWiseModel()
                            {
                                Date = date,
                                ProductInformation = _ctx.ProductsToUsers
                                    .Where(pu => pu.UserId == user.Id)
                                    .Select(pi => new ProductInfoModel()
                                    {
                                        ProductName = pi.ProductInformation.Name,
                                        TotalWeight = pi.CropData
                                        .Where(cd => cd.DateTime.Month == date.Month && cd.DateTime.Year == date.Year)
                                        .Select(cd => Convert.ToInt64(cd.Weight)).Sum()
                                    })
                                    .ToList()
                            });
                        }
                    }
                }
                return monthWiseData.OrderBy(md => md.Date).ToList();
            }
            catch (Exception ex)
            {
                return monthWiseData;
            }
        }
        public List<DateTime> GetDateRangeOfUserData(string userName)
        {
            List<DateTime> userDates = new List<DateTime>();
            try
            {
                DateTime startDate;
                DateTime endDate;
                if (userName != "")
                {
                    var user = _userManager.FindByNameAsync(userName).Result;
                    startDate = _ctx.CropDatas.Where(cd => cd.ProductsToUser.UserId == user.Id).OrderBy(cd => cd.DateTime).FirstOrDefault().DateTime;
                    endDate = _ctx.CropDatas.Where(cd => cd.ProductsToUser.UserId == user.Id).OrderByDescending(cd => cd.DateTime).FirstOrDefault().DateTime;
                }
                else
                {
                    startDate = _ctx.CropDatas.OrderBy(cd => cd.DateTime).FirstOrDefault().DateTime;
                    endDate = _ctx.CropDatas.OrderByDescending(cd => cd.DateTime).FirstOrDefault().DateTime;
                }
                for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                    userDates.Add(new DateTime(date.Year, date.Month, 1));
                return userDates;
            }
            catch (Exception)
            {
                return userDates;
            }
        }
        public PublicDashboardModel GetDashBoardForPublic()
        {
            PublicDashboardModel publicDashboard = new PublicDashboardModel();
            try
            {
                var userRole = _roleManager.FindByNameAsync("user").Result;
                publicDashboard.TotalUsers = _ctx.SwmUsers.Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).Count();
                publicDashboard.TotalWeight = _ctx.CropDatas.Select(cd => Convert.ToInt64(cd.Weight)).Sum();
                publicDashboard.TotalMachines = _ctx.MachineInformations.Count();
                publicDashboard.TotalProducts = _ctx.ProductInformations.Count();
                publicDashboard.TotalUserLocations = _ctx.UserLocations.Count();
                publicDashboard.LastUserRegisterd = _ctx.SwmUsers.Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).OrderByDescending(u => u.RegisterDate).First().FullName;

                return publicDashboard;
            }
            catch (Exception ex)
            {
                return publicDashboard;
            }
        }
        public List<SearchUserModel> GetAllUsersForPublic(int pageNo)
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
                    .Skip(pageNo == 1 ? 0 : (pageNo * numberOfUsersPerRequest) - 50)
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
        public UserDetailsModel GetUserDetails(string subId)
        {
            UserDetailsModel userDetails = new UserDetailsModel();
            try
            {
                int maxLatestUpadatedDisplayAmmount = 15;
                int counter = 1;
                var subDetails = _ctx.UserToSubscriptions
                    .Where(us => us.SubscriptionId == subId)
                    .Select(us => new { user = us.SwmUser, subType = us.SubscriptionType.Name, subId = us.SubscriptionId })
                    .FirstOrDefault();
                if (subDetails != null)
                {
                    userDetails = _ctx.SwmUsers
                        .Where(u => u.Id == subDetails.user.Id)
                        .Select(u => new UserDetailsModel()
                        {
                            FullName = u.FullName,
                            UserName = u.UserName,
                            SubType = subDetails.subType,
                            TotalLocation = _ctx.UserLocations.Where(ul => ul.UserId == u.Id).Count(),
                            TotalProducts = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).Count(),
                            LastUpdatedProduct = _ctx.CropDatas.Where(cd => cd.ProductsToUser.UserId == u.Id).OrderByDescending(cd => cd.DateTime).Select(cd => cd.ProductsToUser.ProductInformation.Name).FirstOrDefault(),
                            TotalWeight = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).SelectMany(pu => pu.CropData).Sum(cd => Convert.ToInt64(cd.Weight)),
                            ContactNo = u.PhoneNumber,
                            Email = u.Email,
                            SubId = subDetails.subId,
                            IsNewUser = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).Count() == 0,
                            HaveSomeData = _ctx.CropDatas.FirstOrDefault(cd => cd.ProductsToUser.UserId == u.Id) != null,
                            ProductsIntoAccount = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).Select(pu => pu.ProductInformation).ToList(),
                            UserLocations = _ctx.UserLocations.Where(ul => ul.UserId == u.Id).Select(ul => new AddNewLocationModel()
                            {
                                Address = ul.Address,
                                Name = ul.Name,
                                PinNo = ul.PinNo.ToString(),
                                State = ul.State.Name,
                                Country = ul.Country.Name
                            }).ToList(),
                            LatestUpdatedProductsTableInformation = _ctx.CropDatas.Where(cd => cd.ProductsToUser.UserId == subDetails.user.Id).OrderByDescending(cd => cd.DateTime).Take(maxLatestUpadatedDisplayAmmount)
                            .Select(cd => new UserDetailsLatestUpdatedTableModel()
                            {
                                DateAndTime = cd.DateTime,
                                Location = cd.UserLocationToMachine.UserLocation.Address,
                                ProductName = cd.ProductsToUser.ProductInformation.Name,
                                Weight = cd.Weight
                            }).ToList()
                        }).FirstOrDefault();

                    foreach (var datum in userDetails.LatestUpdatedProductsTableInformation)
                        datum.No = counter++;
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                return userDetails;
            }
        }
        public UserDetailsModel GetUserDetailsLight(string subId)
        {
            UserDetailsModel userDetails = new UserDetailsModel();
            try
            {
                var subDetails = _ctx.UserToSubscriptions
                    .Where(us => us.SubscriptionId == subId)
                    .Select(us => new { user = us.SwmUser, subType = us.SubscriptionType.Name, subId = us.SubscriptionId })
                    .FirstOrDefault();
                if (subDetails != null)
                {
                    userDetails = _ctx.SwmUsers
                        .Where(u => u.Id == subDetails.user.Id)
                        .Select(u => new UserDetailsModel()
                        {
                            UserName = u.UserName,
                            TotalLocation = _ctx.UserLocations.Where(ul => ul.UserId == u.Id).Count(),
                            TotalProducts = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).Count(),
                            LastUpdatedProduct = _ctx.CropDatas.Where(cd => cd.ProductsToUser.UserId == u.Id).OrderByDescending(cd => cd.DateTime).Select(cd => cd.ProductsToUser.ProductInformation.Name).FirstOrDefault(),
                            TotalWeight = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).SelectMany(pu => pu.CropData).Sum(cd => Convert.ToInt64(cd.Weight)),
                            IsNewUser = _ctx.ProductsToUsers.Where(pu => pu.UserId == u.Id).Count() == 0,
                            HaveSomeData = _ctx.CropDatas.FirstOrDefault(cd => cd.ProductsToUser.UserId == u.Id) != null,
                        }).FirstOrDefault();
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                return userDetails;
            }
        }
        public int GetTotalUsers()
        {
            try
            {
                var userRole = _roleManager.FindByNameAsync("user").Result;
                return _ctx.SwmUsers.Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).ToList().Count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public string GetSubIdFromUserName(string userName)
        {
            try
            {
                var user = _userManager.FindByNameAsync(userName).Result;
                return _ctx.UserToSubscriptions.FirstOrDefault(us => us.UserID == user.Id).SubscriptionId;
            }
            catch (Exception ex)
            {
                return "";
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
                var userRole = _roleManager.FindByNameAsync("user").Result;
                if (names != null)
                {
                    foreach (var name in names)
                        users.AddRange(
                            _ctx.SwmUsers
                            .Where(u => u.FullName.ToLower().Contains(name.Trim().ToLower()) || u.FullName.ToLower() == name.Trim().ToLower())
                            .Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).ToList()
                        );
                }
                else
                    users = _ctx.SwmUsers.Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).ToList();

                foreach (var user in users.ToList())
                {
                    satisfyProducts = satisfyLocation = satisfyStates = satisfyCountries = 0;
                    var userLocations = _ctx.UserLocations
                        .Where(ul => ul.UserId == user.Id)
                        .Select(ul => new {
                            address = ul.Address,
                            state = ul.State.Name,
                            country = ul.Country.Name
                        })
                        .ToList();
                    if (products != null)
                    {
                        foreach (var product in products)
                        {
                            if (_ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).FirstOrDefault(pu => pu.ProductInformation.Name == product) != null)
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
                        if (userLocations.Where(ul => ul.address.ToLower().Contains(parameters.Location.Trim().ToLower())).FirstOrDefault() != null)
                        {
                            satisfyLocation = 1;
                            break;
                        }
                        if (satisfyLocation == 0)
                            users.Remove(user);
                    }
                    if (states != null)
                    {
                        foreach (var state in states)
                        {
                            if (userLocations.Where(ul => ul.state.ToLower() == state.ToLower().Trim()).FirstOrDefault() != null)
                            {
                                satisfyStates = 1;
                                break;
                            }
                            if (satisfyStates == 0)
                                users.Remove(user);
                        }
                        
                    }
                    if (countries != null)
                    {
                        foreach (var country in countries)
                        {
                            if (userLocations.Where(ul => ul.country.ToLower() == country.ToLower().Trim()).FirstOrDefault() != null)
                            {
                                satisfyCountries = 1;
                                break;
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
        public AdminDashboardModel GetDashBoardForAdmin()
        {
            AdminDashboardModel adminDashboard = new AdminDashboardModel();
            try
            {
                var userRole = _roleManager.FindByNameAsync("user").Result;
                adminDashboard.TotalLocations = _ctx.UserLocations.Count();
                adminDashboard.TotalPorducts = _ctx.ProductInformations.Count();
                adminDashboard.TotalUsers = _ctx.SwmUsers.Where(u => u.Roles.Any(ur => ur.RoleId == userRole.Id)).Count();
                adminDashboard.TotalWeight = _ctx.CropDatas.Select(cd => cd.Weight).Sum();

                return adminDashboard;
            }
            catch (Exception ex)
            {
                return adminDashboard;
            }
        }
    }
}