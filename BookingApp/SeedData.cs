using BookingApp.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp
{
    public static class SeedData
    {
        public static void Seed(UserManager<Client> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        public static void SeedUsers(UserManager<Client> userManager)
        {
            if(userManager.FindByNameAsync("admin@localhost.com").Result == null) 
            {
                var user = new Client
                {
                    UserName = "admin@localhost.com",
                    Email = "admin@localhost.com"
                };
                var result = userManager.CreateAsync(user, "P@ssword1!").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
               var result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Client").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Client"
                };
                var result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
