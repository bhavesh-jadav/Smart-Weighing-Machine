﻿using Microsoft.AspNetCore.Identity;
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
                    new UserLocation()
                    {
                        UserId = user.Id,
                        Name = "Farm 1",
                        Address = "Palghar, Thane",
                        PinNo = 401404,
                        StateId = 1,
                        CountryId = 1
                    },
                    new UserLocation()
                    {
                        UserId = user.Id,
                        Name = "Farm 2",
                        Address = "Bhuj, Kutch",
                        PinNo = 370001,
                        StateId = 2,
                        CountryId = 1
                    }
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
                    new ProductInformation { Name = "Sesame" },
                    new ProductInformation { Name = "Cumin" }
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
