using Bogus;
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

                //await SeedUser(userManager);

                //for (int i = 0; i < 50; i++)
                //{
                //    await SeedOnlOrders(db);
                //}

                //for (int i = 0; i < 50; i++)
                //{
                //    await SeedTableOrders(db);
                //}

            }
            return;
        }
        private static async Task SeedUser(UserManager<ApplicationUser> userManager)
        {
            // Sử dụng Faker để tạo dữ liệu ngẫu nhiên
            var faker = new Faker<ApplicationUser>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, (f, u) => $"{u.UserName}@example.com")
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("##########"))
                .RuleFor(u => u.StreetAddress, f => f.Address.StreetAddress())
                .RuleFor(u => u.State, f => f.Address.State())
                .RuleFor(u => u.City, f => f.Address.City());

            // Tạo 10 user với Role_OnlCustomer
            for (int i = 0; i < 10; i++)
            {
                var user = faker.Generate();
                await userManager.CreateAsync(user, "Customer123*");
                await userManager.AddToRoleAsync(user, SD.Role_OnlCustomer);
            }

            // Tạo 5 user với Role_Staff
            for (int i = 0; i < 5; i++)
            {
                var user = faker.Generate();
                await userManager.CreateAsync(user, "Staff123*");
                await userManager.AddToRoleAsync(user, SD.Role_Staff);
            }

            // Tạo 20 user với Role_Table
            for (int i = 1; i <= 20; i++)
            {
                var tableUser = new ApplicationUser
                {
                    UserName = $"Table{i}",
                    Email = $"table{i}@restaurant.com",
                    Name = $"Table {i}",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 Restaurant St.",
                    State = "Same State",
                    City = "Restaurant City"
                };
                await userManager.CreateAsync(tableUser, "Table123*");
                await userManager.AddToRoleAsync(tableUser, SD.Role_Table);
            }

            // Tạo thêm 2 user với Role_Admin
            for (int i = 0; i < 2; i++)
            {
                var user = faker.Generate();
                await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, SD.Role_Admin);
            }
        }
        public static async Task SeedOnlOrders(ApplicationDbContext dbContext)
        {
            // Danh sách UserId có Role là OnlineCustomer
            var userIds = new List<string>
            {
                "692da361-36d4-43ae-8731-2fd31e34dca5",
                "8223bd21-1949-461b-8b46-0720519b734b",
                "85b44da8-87e5-4ab6-9f00-e7a61155a78c",
                "9a76670a-3ee9-47fc-88db-1de5201bc5a4",
                "aea6895e-897d-4a41-a17c-fa19b90cd4dd",
                "bfbda3cb-22d8-4ed9-8d32-9c95f6b8555b",
                "c0959ed6-f57e-427e-8cc3-b59b4ce60228",
                "c1735be6-5ec6-4852-b7e3-3ed9c0092beb",
                "f9cb8e53-f37c-4496-8d05-e5d0d2b0056f",
                "fccafed9-e04b-4085-8221-2f8e6ef3f046"
            };

            var random = new Random();
            var menuItemIds = Enumerable.Range(1, 10).ToList(); // MenuItem Id từ 1 đến 10

            foreach (var userId in userIds)
            {
                // Tạo OrderHeader cho mỗi UserId
                var orderHeader = new OrderHeader
                {
                    ApplicationUserId = userId,
                    OrderDate = DateTime.Now.AddDays(-random.Next(30)), // Ngày ngẫu nhiên trong tháng
                    OrderStatus = random.Next(2) == 0 ? OrderStatus.Completed : OrderStatus.Cancelled,
                    DeliveryInfo = new DeliveryInfo
                    {
                        Name = $"Customer {random.Next(1000, 9999)}",
                        PhoneNumber = $"090{random.Next(1000000, 9999999)}",
                        StreetAddress = $"Street {random.Next(1, 100)}",
                        City = "CityName",
                        State = "StateName"
                    }
                };

                // Tạo danh sách OrderDetail ngẫu nhiên cho OrderHeader
                var orderDetails = new List<OrderDetail>();
                int totalItems = 0;
                double orderTotal = 0;

                int detailCount = random.Next(1, 5); // Số lượng sản phẩm từ 1 đến 5
                for (int i = 0; i < detailCount; i++)
                {
                    int menuItemId = menuItemIds[random.Next(menuItemIds.Count)];
                    int quantity = random.Next(1, 6); // Số lượng từ 1 đến 5
                    double price = random.Next(5, 21); // Giá từ 5 đến 20

                    orderDetails.Add(new OrderDetail
                    {
                        MenuItemId = menuItemId,
                        Quantity = quantity,
                        Price = price
                    });

                    totalItems += quantity;
                    orderTotal += quantity * price;
                }

                orderHeader.ItemsTotal = totalItems;
                orderHeader.OrderTotal = orderTotal;
                orderHeader.DeliveryFee = orderTotal > 100 ? 0 : 2;

                orderHeader.OrderDetail = orderDetails;

                // Thêm OrderHeader vào context
                dbContext.OrderHeaders.Add(orderHeader);
            }

            // Lưu thay đổi vào database
            await dbContext.SaveChangesAsync();
        }
        public static async Task SeedTableOrders(ApplicationDbContext dbContext)
        {
            // Danh sách UserId có Role là Table
            var tableUserIds = new List<string>
            {
                "0ba7db8c-565e-4a24-a1d7-88bf486ab170",
                "129e378e-0ffe-4f88-a5de-73a4760b9ad9",
                "193bf4ac-7126-41fc-bbdb-d6943dba1ad7",
                "1b69596b-8a71-48db-89ff-e88d777331a4",
                "37a157f7-e033-488a-ab30-a69d4198c891",
                "65ab30fa-1dc0-4fcd-897b-2a700454b18b",
                "66063959-4638-4021-abb1-56251de81ae2",
                "6c8d1335-99aa-49bf-8ea7-dcc18d68e61a",
                "6dabbdac-d31c-4c0c-9027-e76563341f0f",
                "75037f14-6959-4641-b17a-d1f761766c68",
                "84b7a5d8-9bf1-4381-9339-b587752dd80a",
                "9da20e90-45ff-457a-9552-e21009c00d6d",
                "a81e6ee6-dbbd-4b4b-8549-287560f858cf",
                "a991a855-fb20-4ec4-9482-4b4e86dcda73",
                "aa8330bb-0c63-49d9-a671-215b47d3e081",
                "ae402cbb-f2b1-44ce-a062-49d78a41ce52",
                "af6ba506-55cd-4204-acce-b87b257ead68",
                "b26388da-679e-499c-8b7e-2afab295dd02",
                "c0b3c0eb-accf-4e27-838e-2eaf6a400a17",
                "f82f6813-7f3e-40d3-957a-96844675615a"
            };

            var random = new Random();
            var menuItemIds = Enumerable.Range(1, 10).ToList(); // MenuItem Id từ 1 đến 10

            foreach (var userId in tableUserIds)
            {
                // Tạo OrderHeader cho mỗi UserId với vai trò là Table
                var orderHeader = new OrderHeader
                {
                    ApplicationUserId = userId,
                    OrderDate = DateTime.Now.AddDays(-random.Next(30)), // Ngày ngẫu nhiên trong tháng
                    OrderStatus = random.Next(2) == 0 ? OrderStatus.Completed : OrderStatus.Cancelled,
                    DeliveryInfo = null, // DeliveryInfo null vì khách ăn tại chỗ
                    DeliveryFee = 0 // Không mất phí giao hàng
                };

                // Tạo danh sách OrderDetail ngẫu nhiên cho OrderHeader
                var orderDetails = new List<OrderDetail>();
                int totalItems = 0;
                double orderTotal = 0;

                int detailCount = random.Next(1, 5); // Số lượng sản phẩm từ 1 đến 5
                for (int i = 0; i < detailCount; i++)
                {
                    int menuItemId = menuItemIds[random.Next(menuItemIds.Count)];
                    int quantity = random.Next(1, 6); // Số lượng từ 1 đến 5
                    double price = random.Next(5, 21); // Giá từ 5 đến 20

                    orderDetails.Add(new OrderDetail
                    {
                        MenuItemId = menuItemId,
                        Quantity = quantity,
                        Price = price
                    });

                    totalItems += quantity;
                    orderTotal += quantity * price;
                }

                orderHeader.ItemsTotal = totalItems;
                orderHeader.OrderTotal = orderTotal;
                orderHeader.OrderDetail = orderDetails;

                // Thêm OrderHeader vào context
                dbContext.OrderHeaders.Add(orderHeader);
            }

            // Lưu thay đổi vào database
            await dbContext.SaveChangesAsync();
        }

    }
}
