using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Areas.Identity;
using Store.Models;

namespace Store.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            await using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>());

            await InitUsersAsync(serviceProvider);
            await context.InitContactUsMessages();
            await context.InitProductsAndCategories();

        }

        private static async Task InitUsersAsync(IServiceProvider serviceProvider)
        {
            var adminId = await EnsureUser(serviceProvider, "admin@store.com", "admin@store.com");
            await EnsureRole(serviceProvider, adminId, Roles.Admin);

            await EnsureUser(serviceProvider, "user@user.com", "user@user.com");
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager!.FindByNameAsync(userName);
            if (user is null)
            {
                user = new IdentityUser
                {
                    Email = userName,
                    UserName = userName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user is null)
                throw new Exception("The password is probably not strong enough!");

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager is null)
                throw new Exception("roleManager null");

            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            if (userManager is null)
                throw new Exception("userManager is null");

            var user = await userManager.FindByIdAsync(uid);
            if (user is null)
                throw new Exception($"The user with id ({uid}) does not exist.");

            var identityResult = await userManager.AddToRoleAsync(user, role);
            return identityResult;
        }

        private static async Task InitContactUsMessages(this ApplicationDbContext context)
        {
            if (context.Messages.Any())
            {
                return;
            }

            context.Messages.AddRange(
                new ContactUsMessage
                {
                    Name = "Daniel Boncica",
                    Email = "daniel@example.com",
                    Title = "Improvement Suggestions",
                    Message = "Hello, I have some suggestions for improving the website. Can we discuss them?"
                },
                new ContactUsMessage
                {
                    Name = "Sara Ashty",
                    Email = "sara@example.com",
                    Title = "More Information",
                    Message = "Hello, I wanted to get more information about your products and their prices. Can you help?"
                },
                new ContactUsMessage
                {
                    Name = "Alonso Ramirez",
                    Email = "ali@example.com",
                    Title = "Login Problem",
                    Message = "Hello, I can't log in to my account. Can you help?"
                },
                new ContactUsMessage
                {
                    Name = "James Bomd",
                    Email = "jamesnegin@example.com",
                    Title = "Collaboration Proposal",
                    Message = "Hello, I'm interested in collaborating with you. Can we discuss this?"
                },
                new ContactUsMessage
                {
                    Name = "Anna Bomb",
                    Email = "Anna@example.com",
                    Title = "Feedback on New Product",
                    Message = "Hello, your new product looks very attractive. I'll share my feedback on it."
                },
                
                new ContactUsMessage
                {
                    Name = "Zara Micky",
                    Email = "zara@example.com",
                    Title = "Critiques and Suggestions",
                    Message = "Hello, I have some critiques and suggestions that I think can help improve your service quality."
                }
            );
            await context.SaveChangesAsync();
        }

        private static async Task InitProductsAndCategories(this ApplicationDbContext context)
        {
            if (context.Categories.Any() || context.Products.Any())
                return;

            var categories = new List<Category>
            {
                new() { Name = "BMW" },
                new() { Name = "BENTLEY" },
                new() { Name = "PORSCHE" },
                new() { Name = "FERRARI" },
                new() { Name = "LAND ROVER" },
                new() { Name = "PRE ORDER" },
            };
            context.Categories.AddRange(categories);

            context.Products.AddRange(
                new Product
                {
                    Name = "BMW",
                    Description = "On track for solid growth in 2023: Dynamic BEV growth and global sales increase Q3.",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[0] },
                        new() { Category = categories[5] }
                    },
                    Price = 125000,
                    StockQuantity = 50,
                    Image = "/img/products/bmw.jpeg",
                    ProductWeight = 3050.00,
                },
                new Product
                {
                    Name = "Bentley",
                    Description = "On track for solid growth in 2023: Dynamic BEV growth and global sales increase Q3.",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                    Price = 1800,
                    StockQuantity = 9,
                    Image = "/img/products/bentley.jpeg",
                    ProductWeight = 3055.50,
                },
   
                new Product
                {
                    Name = "Porsche",
                    Description = "On track for solid growth in 2023: Dynamic BEV growth and global sales increase Q3.",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                    Price = 100000,
                    StockQuantity = 25,
                    Image = "/img/products/porsche.jpeg",
                    ProductWeight = 2070.7,
                },
                new Product
                {
                    Name = "Ferrari",
                    Description = "On track for solid growth in 2023: Dynamic BEV growth and global sales increase Q3.",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[3] },
                        new() { Category = categories[5] }
                    },
                    Price = 150000,
                    StockQuantity = 60,
                    Image = "/img/products/ferrari.jpeg",
                    ProductWeight = 2070.3,
                },
                new Product
                {
                    Name = "Land Rover",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 128000,
                    StockQuantity = 100,
                    Image = "/img/products/landrover.jpeg",
                    ProductWeight = 3070.1,
                },
               
                new Product
                {
                    Name = "Preorder",
                    Description = "On track for solid growth in 2023: Dynamic BEV growth and global sales increase Q3.",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[4] },
                        new() { Category = categories[5] }
                    },
                    Price = 200000,
                    StockQuantity = 25,
                    Image = "/img/products/lamborghini.jpeg",
                    ProductWeight = 3550.6
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
