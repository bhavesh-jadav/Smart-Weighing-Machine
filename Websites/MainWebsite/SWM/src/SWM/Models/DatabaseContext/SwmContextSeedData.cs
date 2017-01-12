using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWM.Models
{
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
            if(!_ctx.States.Any())
            {
                State[] states = { new State { Name = "Maharashtra" }, new State { Name = "Gujarat" } };
                _ctx.States.AddRange(states);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.Countries.Any())
            {
                var con = new Country() { Name = "India" };
                _ctx.Countries.Add(con);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.PinNumbers.Any())
            {
                var pin = new PinNumber() { Pin = 400097 };
                _ctx.PinNumbers.Add(pin);
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
            if (await _userManager.FindByEmailAsync("abc@lolol.com") == null)
            {
                var user = new SwmUser()
                {
                    UserName = "Bhavesh",
                    Email = "abc@lolol.com",
                    FullName = "Bhavesh Jadav",
                    PhoneNumber = "+918888888888"
                };

                await _userManager.CreateAsync(user, "bhavesh123");

                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (await _userManager.FindByEmailAsync("xyz@lolol.com") == null)
            {
                var user = new SwmUser()
                {
                    UserName = "Kaushal",
                    Email = "xyz@lolol.com",
                    FullName = "Kaushal Mania",
                    PhoneNumber = "+918888888888"
                };

                await _userManager.CreateAsync(user, "kaushal123");

                if (!await _userManager.IsInRoleAsync(user, "user"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }
            }

            if (!_ctx.UserLocations.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var state = _ctx.States.FirstOrDefault(s => s.Name == "Maharashtra");
                var con = _ctx.Countries.FirstOrDefault(c => c.Name == "India");
                var pin = _ctx.PinNumbers.FirstOrDefault(p => p.Pin == 400097);

                var floc = new UserLocation()
                {
                    UserId = user.Id,
                    Name = "Farm 1",
                    Address = "Gandhi Nagar, Thane",
                    PinId = pin.Id,
                    StateId = state.Id,
                    CountryId = con.Id
                };
                _ctx.UserLocations.Add(floc);
                await _ctx.SaveChangesAsync();
            }
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
                await _ctx.SaveChangesAsync();
            }

            if (!_ctx.OtherDatas.Any())
            {
                OtherData[] otherdatas =
                {
                    new OtherData(){ Name = "SubscriptionCount", Value = "99" },
                    new OtherData(){ Name = "WebEnv", Value = "Development" }
                };
                _ctx.OtherDatas.AddRange(otherdatas);
                await _ctx.SaveChangesAsync();
            }

            if (!_ctx.UserToSubscriptions.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var scount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "SubscriptionCount");
                int count = Convert.ToInt16(scount.Value);
                count++;
                var subtype = _ctx.SubscriptionTypes.FirstOrDefault(s => s.Name == "Farming");

                var utos = new UserToSubscription()
                {
                    UserID = user.Id,
                    SubscriptionTypeId = subtype.Id,
                    SubscriptionId = count
                };

                _ctx.UserToSubscriptions.Add(utos);
                scount.Value = count.ToString();
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.ProductInformations.Any())
            {
                ProductInformation[] cinfos = {new ProductInformation { Name = "Apple"}, new ProductInformation { Name = "Mango" }, new ProductInformation { Name = "Spinach" }, new ProductInformation { Name = "Grapes"}};
                _ctx.ProductInformations.AddRange(cinfos);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.ProductsToUsers.Any())
            {
                //Used by crop data table and data will be added directly into this table after checking if it exists or not
                //verify before entering data into this table
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                ProductsToUser[] ctou = {new ProductsToUser { ProductID = 1, UserId = user.Id}, new ProductsToUser { ProductID = 2, UserId = user.Id },
                new ProductsToUser { ProductID = 3, UserId = user.Id}, new ProductsToUser { ProductID = 4, UserId = user.Id}};
                _ctx.AddRange(ctou);
                await _ctx.SaveChangesAsync();
            }
            if(!_ctx.UserLocationToMachines.Any())
            {
                //Used by crop data table and data will be added directly into this table after checking if it exists or not
                //verify before entering data into this table
                var ftom = new UserLocationToMachine() { UserLocationId = 1, MachineId = 70000 };
                _ctx.UserLocationToMachines.Add(ftom);
                await _ctx.SaveChangesAsync();
            }

            if(!_ctx.CropDatas.Any())
            {
                //before adding data into this table verify data in CropsToUser and FarmlocationToMachine tables
                DateTime datetime = new DateTime(2016, 9, 30, 21, 20, 20);
                CropData[] cdatas =
                {
                    new CropData {CropToUserId = 1, DateTime = datetime.AddMinutes(1), UserLocationToMachineId = 1, Weight = 55 },
                    new CropData {CropToUserId = 2, DateTime = datetime.AddMinutes(2), UserLocationToMachineId = 1, Weight = 100 },
                    new CropData {CropToUserId = 3, DateTime = datetime.AddMinutes(3), UserLocationToMachineId = 1, Weight = 20 },
                    new CropData {CropToUserId = 4, DateTime = datetime.AddMinutes(4), UserLocationToMachineId = 1, Weight = 123 },
                    new CropData {CropToUserId = 2, DateTime = datetime.AddMinutes(5), UserLocationToMachineId = 1, Weight = 555 },
                    new CropData {CropToUserId = 3, DateTime = datetime.AddMinutes(6), UserLocationToMachineId = 1, Weight = 525 },
                    new CropData {CropToUserId = 1, DateTime = datetime.AddMinutes(7), UserLocationToMachineId = 1, Weight = 65 },
                    new CropData {CropToUserId = 1, DateTime = datetime.AddMinutes(8), UserLocationToMachineId = 1, Weight = 512 },
                    new CropData {CropToUserId = 2, DateTime = datetime.AddMinutes(9), UserLocationToMachineId = 1, Weight = 201 },
                    new CropData {CropToUserId = 4, DateTime = datetime.AddMinutes(10), UserLocationToMachineId = 1, Weight = 215 }
                };
                _ctx.AddRange(cdatas);
                await _ctx.SaveChangesAsync();
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
