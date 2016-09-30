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
                    Email = "abc@lolol.com"
                };

                await _userManager.CreateAsync(user, "bhavesh");

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
                    Email = "xyz@lolol.com"
                };

                await _userManager.CreateAsync(user, "kaushal");

                if (!await _userManager.IsInRoleAsync(user, "user"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }
            }

            if (!_ctx.FarmLocations.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var state = _ctx.States.FirstOrDefault(s => s.Name == "Maharashtra");
                var con = _ctx.Countries.FirstOrDefault(c => c.Name == "India");
                var pin = _ctx.PinNumbers.FirstOrDefault(p => p.Pin == 400097);

                var floc = new FarmLocation()
                {
                    UserId = user.Id,
                    Name = "Farm 1",
                    Address = "Gandhi Nagar, Thane",
                    PinId = pin.Id,
                    StateId = state.Id,
                    CountryId = con.Id
                };
                _ctx.FarmLocations.Add(floc);
                await _ctx.SaveChangesAsync();
            }
            if (!_ctx.MachineIds.Any())
            {
                var mid = new MachineId() { Id = 70000, IsAssigned = true };
                _ctx.MachineIds.Add(mid);
                await _ctx.SaveChangesAsync();
            }
            if (!_ctx.MachineToUsers.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var mid = _ctx.MachineIds.FirstOrDefault(m => m.Id == 70000);
                var mtou = new MachineToUser() { UserID = user.Id, MachineId = mid.Id };
                _ctx.MachineToUsers.Add(mtou);
                await _ctx.SaveChangesAsync();
            }

            if (!_ctx.OtherDatas.Any())
            {
                var otherData = new OtherData()
                {
                    Name = "SubscriptionCount",
                    Value = "99"
                };
                _ctx.OtherDatas.Add(otherData);
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





            await _ctx.SaveChangesAsync();
        }
    }
}
