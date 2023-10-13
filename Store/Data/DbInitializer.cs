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
                new() { Name = "Composite Doors" },
                new() { Name = "Upvc Windows" },
                new() { Name = "Upvc Doors" },
                new() { Name = "Roof Windows" },
                new() { Name = "Conservatories" },
                new() { Name = "Bespoke" },
            };
            context.Categories.AddRange(categories);

            context.Products.AddRange(
                new Product
                {
                    Name = "Composite Door Unico1",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[0] },
                        new() { Category = categories[5] }
                    },
                    Price = 1250,
                    StockQuantity = 50,
                    Image = "/img/products/door1.jpg",
                    ProductWeight = 50.00,
                },
                new Product
                {
                    Name = "Composite Unico Fly",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 1800,
                    StockQuantity = 9,
                    Image = "/img/products/door2.jpg",
                    ProductWeight = 55.50,
                },
                new Product
                {
                    Name = "Composite Unico EU",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 2200,
                    StockQuantity = 35,
                    Image = "/img/products/door3.jpg",
                    ProductWeight = 66.30,
                },
                new Product
                {
                    Name = "Composite Unico Traditional",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 1000,
                    StockQuantity = 25,
                    Image = "/img/products/door3.jpg",
                    ProductWeight = 70.7,
                },
                new Product
                {
                    Name = "Composte Door Dior",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[0] },
                        new() { Category = categories[5] }
                    },
                    Price = 1500,
                    StockQuantity = 60,
                    Image = "/img/products/door4.jpg",
                    ProductWeight = 70.3,
                },
                new Product
                {
                    Name = "Composite Door Evoque",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 1200,
                    StockQuantity = 100,
                    Image = "/img/products/door5.jpg",
                    ProductWeight = 70.1,
                },
                new Product
                {
                    Name = "Composite Door Modern",
                    Description = "Solid Composite Door, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[0] } },
                    Price = 1800,
                    StockQuantity = 8,
                    Image = "/img/products/door5.jpg",
                    ProductWeight = 70.2,
                },
                new Product
                {
                    Name = "Upvc Windows",
                    Description = "Upvc Windows made to measure, Fire Resistant, 10 Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                    Price = 500,
                    StockQuantity = 90,
                    Image = "/img/products/window1.gif",
                    ProductWeight = 70.12,
                },
                    new Product
                    {
                        Name = "Upvc Windows 2",
                        Description = "Upvc Windows made to measure, Fire Resistant, 10 Years Warranty, Supply Only",
                        ProductCategories = new List<ProductCategory> { new() { Category = categories[1] } },
                        Price = 650,
                        StockQuantity = 90,
                        Image = "/img/products/window2.gif",
                        ProductWeight = 50.12,
                    },
                new Product
                {
                    Name = "Upvc Door Standard",
                    Description = "Upvc Door made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                    Price = 800,
                    StockQuantity = 30,
                    Image = "/img/products/door8.jpg",
                    ProductWeight = 81.0
                },
                new Product
                {
                    Name = "Upvc Door Modern",
                    Description = "Upvc Door made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[2] } },
                    Price = 900,
                    StockQuantity = 30,
                    Image = "/img/products/door9.jpg",
                    ProductWeight = 81.0
                },
                new Product
                {
                    Name = "Bespoke Upvc Door",
                    Description = "Upvc Door made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[2] },
                        new() { Category = categories[5] }
                    },
                    Price = 900,
                    StockQuantity = 40,
                    Image = "/img/products/door10.jpg",
                    ProductWeight = 80.5,
                },
                new Product
                {
                    Name = "Roof Windows",
                    Description = "Roof Windows made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[3] } },
                    Price = 1000,
                    StockQuantity = 60,
                    Image = "/img/products/roof1.jpg",
                    ProductWeight = 100.4
                },
                new Product
                {
                    Name = "Roof Windows Conservatory",
                    Description = "Upvc Roof Windows made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[3] },
                        new() { Category = categories[5] }
                    },
                    Price = 750,
                    StockQuantity = 6,
                    Image = "/img/products/roof2.jpg",
                    ProductWeight = 92.5,
                },
                new Product
                {
                    Name = "Conservatories",
                    Description = "Full Conservatories made to measure, Fire Resistant, ten Years Warranty, Supply Only.",
                    ProductCategories = new List<ProductCategory> { new() { Category = categories[4] } },
                    Price = 35000,
                    StockQuantity = 15,
                    Image = "/img/products/Conservatory1.jpg",
                    ProductWeight = 2551.2
                },
                
                new Product
                {
                    Name = "Bespoke Conservaories",
                    Description = "Bespoke Conservatories made to measure, Fire Resistant, ten Years Warranty, Supply Only",
                    ProductCategories = new List<ProductCategory>
                    {
                        new() { Category = categories[4] },
                        new() { Category = categories[5] }
                    },
                    Price = 20000,
                    StockQuantity = 25,
                    Image = "/img/products/Conservatory1.jpg",
                    ProductWeight = 3550.6
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
