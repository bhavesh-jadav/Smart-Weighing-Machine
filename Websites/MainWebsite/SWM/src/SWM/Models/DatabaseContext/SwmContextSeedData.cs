using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWM.Models
{
    public class SwmContextSeedData
    {
        private IConfigurationRoot _config;
        private SwmContext _ctx;
        private RoleManager<UserRoleManager> _roleManager;
        private UserManager<SwmUser> _userManager;

        public SwmContextSeedData(SwmContext ctx, UserManager<SwmUser> userManager, RoleManager<UserRoleManager> roleManager, IConfigurationRoot config)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        //this method make sures that there some data available in database when application starts.
        public async Task EnsureSeedData()
        {
            try
            {
                var ran = new Random();
                int max_indian_test_users = 300; //must be greater than or equal to indian_user_address array
                int max_user_name_repetition = 2;
                int max_user_locations_per_user = 3;
                string[] indian_names = new string[] { };
                Address[] indian_user_address = new Address[] { };
                Address[] indian_farm_address = new Address[] { };

                string[] products = { "Rice", "Wheat", "Peanut", "Cotton", "Millet", "Tea", "Jute", "Hemp", "Coffee", "Orange",
                "Apple", "Barley", "Ragi", "Potato", "Tomato", "Dates", "Cardamom", "Linseed", "Mulberry", "Tobacco" };

                string[] country_names = { "India" };
                string[] sub_type = { "Farming" };
                string[] roles = { "admin", "user", "testuser" };

                try
                {
                    if (_userManager.Users.Count() == 0)
                    {
                        Address[] add = await populateUserAddressFieldFromCsv(@"RawTestData/IndiaPincodeData.csv");
                        indian_names = await populateUserFullNamesFieldFromCsv(@"RawTestData/FullName.csv");
                        indian_user_address = add.Take(max_indian_test_users).ToArray();
                        if((max_user_locations_per_user * max_indian_test_users) < (add.Length - max_indian_test_users))
                            indian_farm_address = add.Skip(max_indian_test_users).Take(max_user_locations_per_user*max_indian_test_users).ToArray();
                        else
                            indian_farm_address = add.Skip(max_indian_test_users).ToArray();
                    }

                }
                catch (Exception ex)
                {
                    //log error
                }

                if (!_ctx.States.Any())
                {
                    List<State> States = new List<State>(); ;
                    foreach (var address in indian_user_address)
                        if (address != null ? States.FirstOrDefault(s => s.Name.Trim().ToLower() == address.state.Trim().ToLower()) == null : false)
                            States.Add(new State() {Name = address.state.Trim() });
                        
                    foreach (var address in indian_farm_address)
                        if (address != null ? States.FirstOrDefault(s => s.Name.Trim().ToLower() == address.state.Trim().ToLower()) == null : false)
                            States.Add(new State() { Name = address.state.Trim() });
                
                    _ctx.States.AddRange(States.ToArray());
                    await _ctx.SaveChangesAsync();
                }
                if (!_ctx.Countries.Any())
                {
                    foreach (var country in country_names)
                        if (_ctx.Countries.FirstOrDefault(c => c.Name.Trim().ToLower() == country.Trim().ToLower()) == null)
                        {
                            _ctx.Countries.Add(new Country() { Name = country.Trim() });
                            _ctx.SaveChanges();
                        }
                    await _ctx.SaveChangesAsync();
                }
                if (!_ctx.SubscriptionTypes.Any())
                {
                    foreach (var sub in sub_type)
                        if (_ctx.SubscriptionTypes.FirstOrDefault(s => s.Name.Trim().ToLower() == sub.Trim().ToLower()) == null)
                        {
                            _ctx.SubscriptionTypes.Add(new SubscriptionType() { Name = sub.Trim() });
                            _ctx.SaveChanges();
                        }

                    await _ctx.SaveChangesAsync();
                }

                if (_roleManager.Roles.Count() == 0)
                {
                    foreach (var role in roles)
                        if (await _roleManager.FindByNameAsync(role) == null)
                            await _roleManager.CreateAsync(new UserRoleManager() { Name = role.Trim() });
                }

                if (!_ctx.OtherDatas.Any())
                {
                    OtherData[] otherdatas =
                    {
                        new OtherData(){ Name = "UserCounts", Value = "101" },
                        new OtherData(){ Name = "WebEnv", Value = "Development" }
                    };
                    _ctx.OtherDatas.AddRange(otherdatas);
                    await _ctx.SaveChangesAsync();
                }


                Dictionary<string, int> states = _ctx.States.ToDictionary(s => s.Name, s => s.Id);
                Dictionary<string, int> countries = _ctx.Countries.ToDictionary(c => c.Name, c => c.Id);

                if (_userManager.Users.Count() == 0)
                {
                    //adding indian farming users into system
                    int usercounts = Int32.Parse(_ctx.OtherDatas.FirstOrDefault(od => od.Name == "UserCounts").Value);
                    var user_name_indexes = Enumerable.Range(0, indian_names.Length).SelectMany(i => Enumerable.Repeat(i, max_user_name_repetition)).OrderBy(i => ran.Next()).ToList().Take(max_indian_test_users);
                    int address_index = 0;
                    foreach (var user_name_index in user_name_indexes)
                    {
                        var user = new SwmUser()
                        {
                            UserName = "farming" + (++usercounts).ToString(),
                            Email = indian_names[user_name_index].Replace(" ", "") + indian_user_address[address_index].phoneNumber.Substring(0, 3) + "@swmw.me",
                            FullName = indian_names[user_name_index],
                            PhoneNumber = "+91" + indian_user_address[address_index].phoneNumber,
                            Address = indian_user_address[address_index].address,
                            PinNo = indian_user_address[address_index].pinNo,
                            StateId = states.FirstOrDefault(s => s.Key.ToLower() == indian_user_address[address_index].state.Trim().ToLower()).Value,
                            CountryId = countries.FirstOrDefault(c => c.Key.ToLower() == "india").Value,
                            RegisterDate = DateTime.Now
                        };
                        await _userManager.CreateAsync(user, indian_names[user_name_index].Split(' ')[0] + _config["seedUsersPasswordKey"]);
                        await _ctx.SaveChangesAsync();
                        await _userManager.AddToRoleAsync(user, "user");
                        await _ctx.SaveChangesAsync();
                        address_index++;
                    }
                    _ctx.OtherDatas.FirstOrDefault(od => od.Name == "UserCounts").Value = usercounts.ToString();
                    await _ctx.SaveChangesAsync();
                }

                //adding test user for public
                if (await _userManager.FindByEmailAsync("pqr@lolol.com") == null)
                {
                    var user = new SwmUser()
                    {
                        UserName = "farming101",
                        Email = "pqr@lolol.com",
                        FullName = "Bhavesh Jadav",
                        PhoneNumber = "+918888888888",
                        Address = "Malad, Mumbai",
                        PinNo = 400097,
                        StateId = 1,
                        CountryId = 1,
                        RegisterDate = DateTime.Now
                    };

                    await _userManager.CreateAsync(user, "bhavesh123");

                    if (!await _userManager.IsInRoleAsync(user, "testuser"))
                        await _userManager.AddToRoleAsync(user, "testuser");
                }

                if (!_ctx.UserLocations.Any())
                {
                    //adding indian user locations. one user can have one or more farm locations.
                    SwmUser[] users = _ctx.SwmUsers.ToArray();
                    int user_location_address_index = 0, user_locations_assigned = 0;
                    for (int i = 0; i < users.Length; i++)
                    {
                        if(user_locations_assigned >= indian_farm_address.Length)
                            user_locations_assigned = user_location_address_index = 0;
                        int no_of_user_locations = ran.Next(1, max_user_locations_per_user+1);
                        user_locations_assigned += no_of_user_locations;
                        if ((indian_farm_address.Length - user_locations_assigned) <= (users.Length - 1 - i))
                            no_of_user_locations = 1;
                        for (int j = 0; j < no_of_user_locations; j++)
                        {
                            var userLocation = new UserLocation()
                            {
                                UserId = users[i].Id,
                                Name = users[i].FullName.Split(' ')[0] + " Farm " + (_ctx.UserLocations.Where(ul => ul.UserId == users[i].Id).Count() + 1).ToString(),
                                Address = indian_farm_address[user_location_address_index].address,
                                PinNo = indian_farm_address[user_location_address_index].pinNo,
                                StateId = states.FirstOrDefault(s => s.Key.ToLower() == indian_farm_address[user_location_address_index].state.Trim().ToLower()).Value,
                                CountryId = countries.FirstOrDefault(c => c.Key.ToLower() == "india").Value
                            };
                            _ctx.UserLocations.Add(userLocation);
                            _ctx.SaveChanges();
                            user_location_address_index++;
                        }
                    }
                    await _ctx.SaveChangesAsync();
                }

                //machine data will be updated by its manufacturer
                if (!_ctx.MachineInformations.Any())
                {
                    var mid = new MachineInformation() { MachineId = 70000, IsAssigned = true, ManufactureDate = DateTime.Now, ManufactureLocation = "Mumbai, India", SellDate = DateTime.Now };
                    _ctx.MachineInformations.Add(mid);
                    await _ctx.SaveChangesAsync();
                }

                if (!_ctx.MachineToUsers.Any())
                {
                    SwmUser[] Users = _ctx.SwmUsers.ToArray();
                    foreach (var user in Users)
                        _ctx.MachineToUsers.Add(new MachineToUser() { UserID = user.Id, MachineId = 70000 });

                    await _ctx.SaveChangesAsync();
                }

                //assigning users to specific subscription
                if (!_ctx.UserToSubscriptions.Any())
                {
                    //adding user to farming subscription
                    int farm_sub_id = _ctx.SubscriptionTypes.FirstOrDefault(st => st.Name == "Farming").Id;
                    SwmUser[] Users = _ctx.SwmUsers.ToArray();
                    foreach (var user in Users)
                    {
                        var utos = new UserToSubscription()
                        {
                            UserID = user.Id,
                            SubscriptionTypeId = farm_sub_id,
                            SubscriptionId = Guid.NewGuid().ToString().Replace("-", "")
                        };
                        _ctx.UserToSubscriptions.Add(utos);
                    }
                    await _ctx.SaveChangesAsync();
                }

                if (!_ctx.ProductInformations.Any())
                {
                    foreach (var product in products)
                        if (_ctx.ProductInformations.FirstOrDefault(pi => pi.Name.ToLower().Trim() == product) == null)
                        {
                            _ctx.ProductInformations.Add(new ProductInformation() { Name = product.Trim() });
                            _ctx.SaveChanges();
                        }
                    await _ctx.SaveChangesAsync();
                }
                if (!_ctx.ProductsToUsers.Any())
                {
                    //Used by crop data table and data will be added directly into this table after checking if it exists or not
                    //verify before entering data into this table

                    //adding farming subscription products to farming subscription users
                    ProductInformation[] productInformation = _ctx.ProductInformations.ToArray();
                    SwmUser[] users = _ctx.SwmUsers.ToArray();
                    foreach (var user in users)
                    {
                        int no_of_products = ran.Next(4, products.Length + 1);
                        var product_indexes = Enumerable.Range(0, products.Length).SelectMany(i => Enumerable.Repeat(i, 1)).OrderBy(i => ran.Next()).ToList().Take(no_of_products);
                        foreach (var product_index in product_indexes)
                        {
                            _ctx.ProductsToUsers.Add(new ProductsToUser()
                            {
                                UserId = user.Id,
                                ProductId = productInformation[product_index].Id
                            });
                        }
                    }

                    await _ctx.SaveChangesAsync();
                }
                if (!_ctx.UserLocationToMachines.Any())
                {
                    //Used by crop data table and data will be added directly into this table after checking if it exists or not
                    //verify before entering data into this table. machine id will be stored in the machine and will be send each time
                    //when new data is entered from the machine

                    UserLocation[] userLocations = _ctx.UserLocations.ToArray();
                    foreach (var userlocation in userLocations)
                    {
                        _ctx.UserLocationToMachines.Add(new UserLocationToMachine()
                        {
                            UserLocationId = userlocation.Id,
                            MachineId = 70000
                        });
                    }
                    await _ctx.SaveChangesAsync();
                }

                if (!_ctx.CropDatas.Any())
                {
                    _ctx.ChangeTracker.AutoDetectChangesEnabled = false;
                    Random random = new Random();
                    int dataSize, min_dataSize = 300, max_dataSize = 500;
                    var users = _ctx.SwmUsers.ToList();
                    var productsToUsers = _ctx.ProductsToUsers.ToList();
                    var userLocations = _ctx.UserLocations.ToList();
                    var userLocationsToMachines = _ctx.UserLocationToMachines.ToList();
                    foreach (var user in users)
                    {
                        dataSize = random.Next(min_dataSize, max_dataSize+1);
                        var ptous = productsToUsers.Where(pu => pu.UserId == user.Id).Select(pu => pu.Id).ToArray();
                        var uls = userLocations.Where(ul => ul.UserId == user.Id).ToList();
                        var ultoms = userLocationsToMachines.Where(um => uls.Any(ul => ul.Id == um.UserLocationId)).Select(um => um.Id).ToArray();

                        //before adding data into this table verify data in ProductsToUser and UserLocationToMachine tables
                        CropData[] cropDatas = new CropData[dataSize];
                        for (int i = 0; i < dataSize; i++)
                            cropDatas[i] = new CropData
                            {
                                ProductToUserId = ptous[random.Next(0, ptous.Length)],
                                DateTime = new DateTime(random.Next(2015, 2017), random.Next(1, 13), random.Next(1, 29), random.Next(0, 23), random.Next(0, 60), random.Next(0, 60)),
                                UserLocationToMachineId = ultoms[random.Next(0, ultoms.Length)],
                                Weight = random.Next(1, 3001)
                            };
                        _ctx.AddRange(cropDatas);
                    }
                    await _ctx.SaveChangesAsync();
                    _ctx.ChangeTracker.AutoDetectChangesEnabled = true;
                }

                //adding admin
                if (await _userManager.FindByEmailAsync("abc@lolol.com") == null)
                {
                    var user = new SwmUser()
                    {
                        UserName = "bhavesh",
                        Email = "abc@lolol.com",
                        FullName = "Bhavesh Jadav",
                        PhoneNumber = "+918888888888",
                        Address = "Malad, Mumbai",
                        PinNo = 400097,
                        StateId = 1,
                        CountryId = 1,
                        RegisterDate = DateTime.Now
                    };

                    await _userManager.CreateAsync(user, _config["adminPass"]);

                    if (!await _userManager.IsInRoleAsync(user, "admin"))
                        await _userManager.AddToRoleAsync(user, "admin");
                }

                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var data = ex.ToString();
                await _ctx.SaveChangesAsync();
            }
        }

        public static string[] Randomize(string[] items)
        {
            Random rand = new Random();

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                string temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
            return items;
        }

        //csv must be in following format
        //PostOfficeName(or)Address,Pincode,City,District,State
        private async Task<Address[]> populateUserAddressFieldFromCsv(string filePath)
        {
            var task = await Task.Run(() =>
            {
                try
                {
                    string[] datas = File.ReadLines(filePath).SelectMany(a => a.Split(';')).ToArray();
                    datas = Randomize(datas);
                    Address[] addresess = new Address[datas.Length];
                    for (int i = 1; i < datas.Length; i++)
                    {
                        var data = datas[i].Split(',');
                        if (data.Length >= 5)
                            addresess[i - 1] = new Address(data[0] + ", " + data[2], int.Parse(data[1].ToString().Trim()), data[4]);
                    }
                    return addresess;
                }
                catch (Exception ex)
                {
                    var data = ex.ToString();
                    return null;
                }
            });
            return task;
        }

        //in format of
        //FirstName,LastName
        private async Task<string[]> populateUserFullNamesFieldFromCsv(string filePath)
        {
            var task = await Task.Run(() =>
            {
                string[] datas = File.ReadLines(filePath).SelectMany(a => a.Split(';')).ToArray();
                datas = Randomize(datas);
                string[] fullNames = new string[datas.Length];
                for (int i = 1; i < datas.Length; i++)
                {
                    var data = datas[i].Split(',');
                    fullNames[i - 1] = $"{data[0]} {data[1]}";
                }
                return fullNames;
            });
            return task;
        }
    }

    internal class Address
    {
        public Address(string address, int pinNo, string state)
        {
            this.address = address;
            this.pinNo = pinNo;
            this.state = state;
            this.phoneNumber = generateRandomNumber();
        }

        public string address { get; set; }
        public int pinNo { get; set; }
        public string state { get; set; }
        public string phoneNumber { get; set; }

        private string generateRandomNumber()
        {
            var random = new Random();
            string number = "";
            for (int i = 0; i < 10; i++)
                number += random.Next(0, 10);
            return number;
        }
    }
}

