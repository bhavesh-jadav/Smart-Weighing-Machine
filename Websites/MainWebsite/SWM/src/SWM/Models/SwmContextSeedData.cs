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
