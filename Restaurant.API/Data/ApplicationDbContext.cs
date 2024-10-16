using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Models;

namespace Restaurant.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MenuItem>().HasData(
                new MenuItem
                {
                    Id = 1,
                    Name = "Spring Roll",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\spring roll.jpg",
                    Price = 7.99,
                    CategoryId = 1, // Assuming 1 is the Id for 'Appetizer'
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 2,
                    Name = "Idli",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\idli.jpg",
                    Price = 8.99,
                    CategoryId = 1, // Assuming 1 is the Id for 'Appetizer'
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 3,
                    Name = "Panu Puri",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\pani puri.jpg",
                    Price = 8.99,
                    CategoryId = 1, // Assuming 1 is the Id for 'Appetizer'
                    SpecialTag = "Best Seller"
                },
                new MenuItem
                {
                    Id = 4,
                    Name = "Hakka Noodles",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\hakka noodles.jpg",
                    Price = 10.99,
                    CategoryId = 2, // Assuming 2 is the Id for 'Entrée'
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 5,
                    Name = "Malai Kofta",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\malai kofta.jpg",
                    Price = 12.99,
                    CategoryId = 2, // Assuming 2 is the Id for 'Entrée'
                    SpecialTag = "Top Rated"
                },
                new MenuItem
                {
                    Id = 6,
                    Name = "Paneer Pizza",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\paneer pizza.jpg",
                    Price = 11.99,
                    CategoryId = 2, // Assuming 2 is the Id for 'Entrée'
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 7,
                    Name = "Paneer Tikka",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\paneer tikka.jpg",
                    Price = 13.99,
                    CategoryId = 2, // Assuming 2 is the Id for 'Entrée'
                    SpecialTag = "Chef's Special"
                },
                new MenuItem
                {
                    Id = 8,
                    Name = "Carrot Love",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\carrot love.jpg",
                    Price = 4.99,
                    CategoryId = 3, // Assuming 3 is the Id for 'Dessert'
                    SpecialTag = ""
                },
                new MenuItem
                {
                    Id = 9,
                    Name = "Rasmalai",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\rasmalai.jpg",
                    Price = 4.99,
                    CategoryId = 3, // Assuming 3 is the Id for 'Dessert'
                    SpecialTag = "Chef's Special"
                },
                new MenuItem
                {
                    Id = 10,
                    Name = "Sweet Rolls",
                    Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                    ImageUrl = "\\images\\menuitem\\sweet rolls.jpg",
                    Price = 3.99,
                    CategoryId = 3, // Assuming 3 is the Id for 'Dessert'
                    SpecialTag = "Top Rated"
                });

            builder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Appetizer",
                    Description = "Light dishes served as a starter"
                },
                new Category
                {
                    Id = 2,
                    Name = "Main Course",
                    Description = "Hearty and filling main dishes"
                },
                new Category
                {
                    Id = 3,
                    Name = "Dessert",
                    Description = "Sweet dishes served after the main meal"
                },
                new Category
                {
                    Id = 4,
                    Name = "Beverage",
                    Description = "Drinks and refreshments"
                },
                new Category
                {
                    Id = 5,
                    Name = "Snack",
                    Description = "Light and quick bites"
                }
            );
        }
    }
}
