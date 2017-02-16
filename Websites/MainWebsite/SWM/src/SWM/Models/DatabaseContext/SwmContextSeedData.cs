using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
    class Address
    {
        public Address(string address, int pinNo, string state)
        {
            this.address = address;
            this.pinNo = pinNo;
            this.state = state;
        }
        public string address { get; set; }
        public int pinNo { get; set; }
        public string state { get; set; }
    }

    public class SwmContextSeedData
    {
        private SwmContext _ctx;
        private RoleManager<UserRoleManager> _roleManager;
        private UserManager<SwmUser> _userManager;

        public SwmContextSeedData(SwmContext ctx, UserManager<SwmUser> userManager, RoleManager<UserRoleManager> roleManager)
        {
            _ctx = ctx;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureSeedData()
        {
            string[] indian_names = { "Mangesh Shah", "Parth Yadav", "Siddharth Nakrani", "Kanishk Jain", "Smith Karmarkar", "Karan Thakkar",
                "Madhav Jain", "Rayan Dsouza", "Jay Pal", "Ritesh Sharma", "Dev Jain", "Nitin Shah", "Harischandra Mehta", "Chinja  Garg"
            };
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
                State[] states = { new State { Name = "Maharashtra" },
                new State { Name = "Gujarat" },
                new State { Name = "Punjab" },
                new State { Name = "Haryana" },
                new State { Name = "Kashmir" },
                new State { Name = "Tamil Nadu" },
                new State { Name = "Rajasthan" },
                new State { Name = "Sikkim" },
                new State { Name = "Bihar" },
                new State { Name = "Uttar Pradesh" }};
                _ctx.States.AddRange(states);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.Countries.Any())
            {
                var con = new Country() { Name = "India" };
                _ctx.Countries.Add(con);
                await _ctx.SaveChangesAsync();
            }
            if (!_ctx.SubscriptionTypes.Any())
            {
                var sub = new SubscriptionType()
                {
                    Name = "Farming"
                };
                _ctx.SubscriptionTypes.Add(sub);
                await _ctx.SaveChangesAsync();
            }
            if (await _roleManager.FindByNameAsync("admin") == null)
            {
                var role = new UserRoleManager()
                {
                    Name = "admin"
                };
                await _roleManager.CreateAsync(role);
            }
            if (await _roleManager.FindByNameAsync("user") == null)
            {
                var role = new UserRoleManager()
                {
                    Name = "user"
                };
                await _roleManager.CreateAsync(role);
            }
            if (await _roleManager.FindByNameAsync("testuser") == null)
            {
                var role = new UserRoleManager()
                {
                    Name = "testuser"
                };
                await _roleManager.CreateAsync(role);
            }
            if (await _userManager.FindByEmailAsync("abc@lolol.com") == null)
            {
                var user = new SwmUser()
                {
                    UserName = "Bhavesh",
                    Email = "abc@lolol.com",
                    FullName = "Bhavesh Jadav",
                    PhoneNumber = "+918888888888",
                    Address = "Malad east, Mumbai",
                    PinNo = 400097,
                    StateId = 1,
                    CountryId = 1,
                    RegisterDate = DateTime.Now
                };

                await _userManager.CreateAsync(user, "bhavesh@9769194780");

                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (await _userManager.FindByEmailAsync("xyz@lolol.com") == null)
            {
                var user = new SwmUser()
                {
                    UserName = "farming102",
                    Email = "xyz@lolol.com",
                    FullName = "Kaushal Mania",
                    PhoneNumber = "+918888888888",
                    Address = "Borivali, Mumbai",
                    PinNo = 400097,
                    StateId = 1,
                    CountryId = 1,
                    RegisterDate = DateTime.Now
                };

                await _userManager.CreateAsync(user, "kaushal123");

                if (!await _userManager.IsInRoleAsync(user, "user"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }
            }

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
                {
                    await _userManager.AddToRoleAsync(user, "testuser");
                }
            }

            if (!_ctx.UserLocations.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                UserLocation[] userLocations =
                {
                    new UserLocation() { UserId = "", Name = "Farm 1", Address = "Palghar, Thane", PinNo = 401404, StateId = 1, CountryId = 1 },
                    new UserLocation() { UserId = "", Name = "Farm 2", Address = "Adivare, Pune", PinNo = 410509, StateId = 2, CountryId = 1 },
                    new UserLocation() { UserId = "", Name = "Farm 3", Address = "Bhatinda, Punjab", PinNo = 151001, StateId = 2, CountryId = 1 },
                    new UserLocation() { UserId = "", Name = "Farm 4", Address = "Baded, Haryana", PinNo = 121104, StateId = 2, CountryId = 1 },
                    new UserLocation() { UserId = "", Name = "Farm 5", Address = "Assar, Kashmir ", PinNo = 182143, StateId = 2, CountryId = 1 },
                };
                _ctx.UserLocations.AddRange(userLocations);

                user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "pqr@lolol.com");
                UserLocation[] userLocations2 =
                {
                    new UserLocation()
                    {
                        UserId = user.Id,
                        Name = "Farm 3",
                        Address = "Vasai Road, Thane",
                        PinNo = 401404,
                        StateId = 1,
                        CountryId = 1
                    },
                    new UserLocation()
                    {
                        UserId = user.Id,
                        Name = "Farm 4",
                        Address = "Junagadh, Kutch",
                        PinNo = 370001,
                        StateId = 2,
                        CountryId = 1
                    }
                };
                _ctx.UserLocations.AddRange(userLocations2);

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
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var mid = _ctx.MachineInformations.FirstOrDefault(m => m.Id == 70000);
                var mtou = new MachineToUser() { UserID = user.Id, MachineId = mid.Id };
                _ctx.MachineToUsers.Add(mtou);

                user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "pqr@lolol.com");
                mid = _ctx.MachineInformations.FirstOrDefault(m => m.Id == 70000);
                mtou = new MachineToUser() { UserID = user.Id, MachineId = mid.Id };
                _ctx.MachineToUsers.Add(mtou);

                await _ctx.SaveChangesAsync();
            }

            if (!_ctx.OtherDatas.Any())
            {
                OtherData[] otherdatas =
                {
                    new OtherData(){ Name = "UserCounts", Value = "100" },
                    new OtherData(){ Name = "WebEnv", Value = "Development" }
                };
                _ctx.OtherDatas.AddRange(otherdatas);
                await _ctx.SaveChangesAsync();
            }

            if (!_ctx.UserToSubscriptions.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "pqr@lolol.com");
                var scount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "UserCounts");
                int count = Convert.ToInt16(scount.Value);
                count++;
                var subtype = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Name == "Farming");

                var utos = new UserToSubscription()
                {
                    UserID = user.Id,
                    SubscriptionTypeId = subtype.Id,
                    SubscriptionId = Guid.NewGuid().ToString().Replace("-", "")
                };
                _ctx.UserToSubscriptions.Add(utos);
                scount.Value = count.ToString();
                await _ctx.SaveChangesAsync();

                user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                scount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "UserCounts");
                count = Convert.ToInt16(scount.Value);
                count++;
                subtype = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Name == "Farming");

                utos = new UserToSubscription()
                {
                    UserID = user.Id,
                    SubscriptionTypeId = subtype.Id,
                    SubscriptionId = Guid.NewGuid().ToString().Replace("-", "")
                };
                _ctx.UserToSubscriptions.Add(utos);
                scount.Value = count.ToString();
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.ProductInformations.Any())
            {
                ProductInformation[] pinfos = {
                                    new ProductInformation { Name = "Rice" },
                                    new ProductInformation { Name = "Wheat" },
                                    new ProductInformation { Name = "Peanut" },
                                    new ProductInformation { Name = "Cotton" },
                                    new ProductInformation { Name = "Millet" },
                                    new ProductInformation { Name = "Tea" },
                                    new ProductInformation { Name = "Jute" },
                                    new ProductInformation { Name = "Hemp" },
                                    new ProductInformation { Name = "Coffee" },
                                    new ProductInformation { Name = "Orange" },
                                    new ProductInformation { Name = "Apple" },
                                    new ProductInformation { Name = "Barley" },
                                    new ProductInformation { Name = "Ragi" },
                                    new ProductInformation { Name = "Potato" },
                                    new ProductInformation { Name = "Tomato" },
                                    new ProductInformation { Name = "Dates" },
                                    new ProductInformation { Name = "Cardamom" },
                                    new ProductInformation { Name = "Linseed" },
                                    new ProductInformation { Name = "Mulberry" },
                                    new ProductInformation { Name = "Tobacco" },
                                };
                _ctx.ProductInformations.AddRange(pinfos);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.ProductsToUsers.Any())
            {
                //Used by crop data table and data will be added directly into this table after checking if it exists or not
                //verify before entering data into this table
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                ProductsToUser[] ptou = {
                    new ProductsToUser { ProductID = 1, UserId = user.Id },
                    new ProductsToUser { ProductID = 2, UserId = user.Id },
                    new ProductsToUser { ProductID = 3, UserId = user.Id },
                    new ProductsToUser { ProductID = 4, UserId = user.Id },
                    new ProductsToUser { ProductID = 5, UserId = user.Id },
                    new ProductsToUser { ProductID = 6, UserId = user.Id },
                    new ProductsToUser { ProductID = 7, UserId = user.Id }
                };
                _ctx.AddRange(ptou);
                await _ctx.SaveChangesAsync();

                user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "pqr@lolol.com");
                ProductsToUser[] ptou2 = {
                    new ProductsToUser { ProductID = 1, UserId = user.Id },
                    new ProductsToUser { ProductID = 2, UserId = user.Id },
                    new ProductsToUser { ProductID = 3, UserId = user.Id },
                    new ProductsToUser { ProductID = 4, UserId = user.Id },
                    new ProductsToUser { ProductID = 5, UserId = user.Id },
                    new ProductsToUser { ProductID = 6, UserId = user.Id },
                    new ProductsToUser { ProductID = 7, UserId = user.Id }
                };
                _ctx.AddRange(ptou2);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.UserLocationToMachines.Any())
            {
                //Used by crop data table and data will be added directly into this table after checking if it exists or not
                //verify before entering data into this table. machine id will be stored in the machine and will be send each time 
                //when new data is entered from the machine

                UserLocationToMachine[] userLocationsToMachines = new UserLocationToMachine[]
                {
                    new UserLocationToMachine() { UserLocationId = 1, MachineId = 70000 },
                    new UserLocationToMachine() { UserLocationId = 2, MachineId = 70000 },
                    new UserLocationToMachine() { UserLocationId = 3, MachineId = 70000 },
                    new UserLocationToMachine() { UserLocationId = 4, MachineId = 70000 }
                };

                _ctx.UserLocationToMachines.AddRange(userLocationsToMachines);
                await _ctx.SaveChangesAsync();
            }

            if(!_ctx.CropDatas.Any())
            {
                Random random = new Random();
                int dataSize = 300;

                var users = _ctx.SwmUsers.ToList().Where(u => _userManager.IsInRoleAsync(u, "user").Result || _userManager.IsInRoleAsync(u, "testuser").Result).ToList();
                foreach (var user in users)
                {
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

            await _ctx.SaveChangesAsync();
        }
    }
}