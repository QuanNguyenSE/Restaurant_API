using Microsoft.AspNetCore.Identity;
using Restaurant.API.Models;
using Restaurant.API.Utility;

namespace Restaurant.API.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (!roleManager.RoleExistsAsync(SD.Role_OnlCustomer).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_OnlCustomer)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Staff)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Table)).GetAwaiter().GetResult();


                //if roles are not created, then we will create admin user as well
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "admin",
                    PhoneNumber = "111222333",
                    StreetAddress = "Nghe An",
                    State = "Vinh Bac Bo",
                    City = "Ha Noi"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@gmail.com");
                userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;
        }
    }
}
