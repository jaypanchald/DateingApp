using Dating.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dating.Repository.Repository
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<User> userManager,
            RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            var json = await System.IO.File.ReadAllTextAsync(@"E:\Angular\DateApp\DateingApp\Dating.Repository\Data\seed.json");
            List<User> userDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<User>>(json);
            if (userDatas == null)
            {
                return;
            }

            var roles = new List<AppRole>
            { 
                new AppRole{ Name= "Member" },
                new AppRole{ Name= "Admin" },
                new AppRole{ Name= "Moderator" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var userData in userDatas)
            {
                userData.UserName = userData.UserName.ToLower();

                await userManager.CreateAsync(userData, "Pa$$w0rd");

                await userManager.AddToRoleAsync(userData, "Member");
            }

            var admin = new User
            {
                UserName = "admin"
            };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Member" });
        }

    }
}
