using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        //this maethod make sures that there some data available in database when application starts.
        public async Task EnsureSeedData()
        {
            try
            {
                var ran = new Random();
                int max_user_name_repetition = 2;

                string[] indian_names = { "Mangesh Shah", "Parth Yadav", "Siddharth Nakrani", "Kanishk Jain", "Smith Karmarkar", "Karan Thakkar",
                    "Madhav Jain", "Rayan Dsouza", "Jay Pal", "Ritesh Sharma", "Dev Jain", "Nitin Shah", "Harischandra Mehta", "Chinja  Garg",
                    "Madhvi Naik", "Sheetal Joshi", "Urmila Jadhav", "Shilpa Mane", "Ayodhya Sharaf", "Ayodhya Sharaf", "Sharvani Shetty"
                };

                string[] products = { "Rice", "Wheat", "Peanut", "Cotton", "Millet", "Tea", "Jute", "Hemp", "Coffee", "Orange",
                "Apple", "Barley", "Ragi", "Potato", "Tomato", "Dates", "Cardamom", "Linseed", "Mulberry", "Tobacco" };

                string[] country_names = { "India" };
                string[] sub_type = { "Farming" };
                string[] roles = { "admin", "user", "testuser" };

                //unique to each and very users. Total number of users will be equal to length of this array
                Address[] indian_user_address =
                {
                    new Address("366, Old No 200, Triplicane High Road", 600005, " Tamil Nadu"),
                    new Address("Gala No.b-40, Kalina Kutir Society, C S T Road, Kalina, Santacruz(e)", 400098, " Maharashtra"),
                    new Address("208 Ansa Industrial Estate, J Saki Vihar Raod, Saki Naka", 400072, " Maharashtra"),
                    new Address("Kripa Nagar, F-6 S V Road, Vile Parle West (west)", 400056, " Maharashtra"),
                    new Address("106/22, R S Puram, Rs Puram", 641002, "Tamil Nadu"),
                    new Address("2-b, New No 3, Ramanathan Street, T Nagar", 600017, " Tamil Nadu"),
                    new Address("A-11 Pruthvi Complex, Opp Azad Halwai, Ashram Road", 380013, " Gujarat"),
                    new Address("Veera Desai, Nr Veera Desai Bus Stop,nr Sharda V, Andheri (west)", 400058, " Maharashtra"),
                    new Address("3rd Floor, 93/95, Sugar House, Kazi Sayeed Street, Masjid Bunder (w)", 400003, "Maharashtra"),
                    new Address("112, Shankar Estate,kevada, Amraiwadi", 380026, "  Gujarat"),
                    new Address("104/5, Gopalreddy, 1, Tavarekere", 560029, " Karnataka"),
                    new Address("1st Floor Balabhavan Central, Avenue Road Nr Diamond Garden, Chembur", 400071, " Maharashtra"),
                    new Address("19/2, Mittal Estate, Andheri Kurla Road, Andheri(e)", 400059, " Maharashtra"),
                    new Address("312, B-wing, Mittal Towers, M G Road", 560001, " Karnataka"),
                    new Address("151, Hilal Manzil, Kazi Sayed Street, Mandvi", 400003, " Maharashtra"),
                    new Address("159, Sanjay 5-b, Mittal Indl Estate, Andheri Kurla Road, Marol Naka, Andheri(e)", 400059, " Maharashtra"),
                    new Address("Unit No 221, H Building, Ansa Industrial Estate, Chandivli Road, Andheri (e),sakinaka, Andheri (west", 400072, " Maharashtra"),
                    new Address("4, Sardar Nagar, S M Rd, Sion Koliwada", 400037, " Maharashtra"),
                    new Address("5203, Sadar Thana Rd, Sadar Bazar", 110006, " Delhi"),
                    new Address("131 Loheki Chawl, 216/218 M.a Road", 400008, " Maharashtra"),
                    new Address("Kanchpada No 2, Ramchandra Lane Extn, Malad(w)", 400064, " Maharashtra"),
                    new Address("7-73/1, Main Road, Kukatpally", 500072, " Andhra Pradesh"),
                    new Address("6100, Gali Batashe Wali, Khari Baoli", 110006, " Delhi")
                };

                //must be greater than user_address array
                Address[] indian_farm_address =
                {
                    new Address(" Mira Bhayander Road", 401104," Maharashtra"),
                    new Address("Commerce Union House, 9, Wallace Street", 400001," Maharashtra"),
                    new Address("159, 5, Amar Industrial Est, Cst Road, Kalina, Santacruz (east)", 400098," Maharashtra"),
                    new Address("Happy Home, Anand Nagar, Dahisar", 400068," Maharashtra"),
                    new Address("105, Sainath Chambers, Sainath Road, Malad (west)", 400064," Maharashtra"),
                    new Address("155, Guru Nanak Auto Market, Kashmere Gate", 110006," Delhi"),
                    new Address("A 57, Part 2, South Extn", 110049," Delhi"),
                    new Address("B 8, 3/4, Balaji Nagar, 90 Feet Rd, Opp New Police Stn,", 400017," Maharashtra"),
                    new Address("Blue Diamond, Fatehgunj", 390002," Gujarat"),
                    new Address("3rd Floor, 61/63 Nagdevi Street, Mandvi", 400003," Maharashtra"),
                    new Address("12, Rajanna Lane, Ganigarpet", 560002," Karnataka"),
                    new Address("Karani Lane, Bhatwadi", 400084," Maharashtra"),
                    new Address("113 Nagdevi Street, Mandvi", 400003," Maharashtra"),
                    new Address("610, Anna Salai, Anna Salai", 600006," Tamil Nadu"),
                    new Address("Plot No.7, Prabhat Centre Annexe, Sec 1a, Belapur (cbd), Navi Mumbai", 400614," Maharashtra"),
                    new Address("11/7, Mathura Road, Faridabad", 121001," Haryana"),
                    new Address("404, Sector 17, Jk Chambers, Vashi", 400705," Maharashtra"),
                    new Address("17 Narasingapuram St, Mount Road", 600002," Tamil Nadu"),
                    new Address("#4009, 100ftrd,jngr,hal2stg Blr-08, Jeevanbimanagar", 560008," Karnataka"),
                    new Address("Metropolitan Mall, Mehrauli Gurgaon Road, Gurgaon, Gurgaon", 122001," Delhi"),
                    new Address("251/53, Fazender House, Ibrahim Rahimtulla Rd, Mandvi", 400003," Maharashtra"),
                    new Address("3-5-1138/1 2, Kachiguda", 500027," Andhra Pradesh"),
                    new Address("1, 1,klsiplyblr-2, Kalasipalyam", 560002," Karnataka"),
                    new Address("#109, Lakdikapul", 500004," Andhra Pradesh"),
                    new Address("Opp. A.s.motors Salatwada, Opp. A.s.motors, Salatwada, Opp. A.s.motors, Salatwada", 390001," Gujarat"),
                    new Address("24/25, Rup Chand Roy Street, Burrabazar", 700007," West Bengal")
                };

                if (!_ctx.States.Any())
                {
                    foreach (var address in indian_user_address)
                        if (_ctx.States.FirstOrDefault(s => s.Name.Trim().ToLower() == address.state.Trim().ToLower()) == null)
                        {
                            _ctx.States.Add(new State() { Name = address.state.Trim() });
                            _ctx.SaveChanges();
                        }
                        
                    foreach (var address in indian_farm_address)
                        if (_ctx.States.FirstOrDefault(s => s.Name.Trim().ToLower() == address.state.Trim().ToLower()) == null)
                        {
                            _ctx.States.Add(new State() { Name = address.state.Trim() });
                            _ctx.SaveChanges();
                        }

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
                    var user_name_indexes = Enumerable.Range(0, indian_names.Length).SelectMany(i => Enumerable.Repeat(i, max_user_name_repetition)).OrderBy(i => ran.Next()).ToList().Take(indian_user_address.Length);
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
                        _ctx.SaveChanges();
                        await _userManager.AddToRoleAsync(user, "user");
                        _ctx.SaveChanges();
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
                    SwmUser[] Users = _ctx.SwmUsers.ToArray();
                    int user_location_address_index = 0, user_locations_assigned = 0;
                    for (int i = 0; i < Users.Length; i++)
                    {
                        int no_of_user_locations = ran.Next(1, 4);
                        user_locations_assigned += no_of_user_locations;
                        if ((indian_farm_address.Length - user_locations_assigned) <= (Users.Length - 1 - i))
                            no_of_user_locations = 1;
                        for (int j = 0; j < no_of_user_locations; j++)
                        {
                            var userLocation = new UserLocation()
                            {
                                UserId = Users[i].Id,
                                Name = Users[i].FullName.Split(' ')[0] + "Farm"+ " " + (_ctx.UserLocations.Where(ul => ul.UserId == Users[i].Id).Count() + 1).ToString(),
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
                    var mid = new MachineInformation() { Id = 70000, IsAssigned = true, ManufactureDate = DateTime.Now, ManufactureLocation = "Mumbai, India", SellDate = DateTime.Now };
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
                                ProductID = productInformation[product_index].Id
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
                    Random random = new Random();
                    int dataSize;

                    var users = _ctx.SwmUsers.ToList();
                    foreach (var user in users)
                    {
                        dataSize = random.Next(300, 501);
                        var ptous = _ctx.ProductsToUsers.Where(pu => pu.UserId == user.Id).Select(pu => pu.Id).ToArray();
                        var uls = _ctx.UserLocations.Where(ul => ul.UserId == user.Id).ToList();
                        var utoms = _ctx.UserLocationToMachines.Where(um => uls.Any(ul => ul.Id == um.UserLocationId)).Select(um => um.Id).ToArray();

                        //before adding data into this table verify data in ProductsToUser and UserLocationToMachine tables
                        CropData[] cropDatas = new CropData[dataSize];
                        for (int i = 0; i < dataSize; i++)
                            cropDatas[i] = new CropData
                            {
                                CropToUserId = ptous[random.Next(0, ptous.Length)],
                                DateTime = new DateTime(random.Next(2015, 2017), random.Next(1, 13), random.Next(1, 29), random.Next(0, 23), random.Next(0, 60), random.Next(0, 60)),
                                UserLocationToMachineId = utoms[random.Next(0, utoms.Length)],
                                Weight = random.Next(1, 3001)
                            };
                        _ctx.AddRange(cropDatas);
                        await _ctx.SaveChangesAsync();
                    }
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
                await _ctx.SaveChangesAsync();
            }
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