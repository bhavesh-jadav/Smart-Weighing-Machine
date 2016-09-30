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
            if(await _roleManager.FindByNameAsync("admin") == null)
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
                    UserName = "Bhavesh Jadav",
                    Email = "abc@lolol.com"
                };

                await _userManager.CreateAsync(user, "Bhavesh");
                if (!await _userManager.IsInRoleAsync(user, "admin"))
                {
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }

            if (await _userManager.FindByEmailAsync("xyz@lolol.com") == null)
            {
                var user = new SwmUser()
                {
                    UserName = "Kaushal Mania",
                    Email = "xyz@lolol.com"
                };

                await _userManager.CreateAsync(user, "Kaushal");
                if (!await _userManager.IsInRoleAsync(user, "user"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }
            }

            if (_ctx.OtherDatas.Any())
            {
                var otherData = new OtherData()
                {
                    Name = "SubscriptionCount",
                    Value = "99"
                };
                _ctx.OtherDatas.Add(otherData);
            }

            if (_ctx.UserToSubscriptions.Any())
            {
                var user = _ctx.SwmUsers.FirstOrDefault(u => u.Email == "xyz@lolol.com");
                var scount = _ctx.OtherDatas.FirstOrDefault(c => c.Name == "SubscriptionCount");
                int count = Convert.ToInt16(scount.Value);
                count++;

            }

            await _ctx.SaveChangesAsync();
        }
    }
}
