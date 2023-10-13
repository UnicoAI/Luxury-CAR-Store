using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ContactUsMessage> Messages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)  // Assuming that Order has a reference to a Product
                .WithMany(p => p.Orders)  // Use the navigation property in the Product entity that represents the collection of orders
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);  // Specify the delete behavior here

            modelBuilder.Entity<ProductCategory>()
                .HasKey(productCategory => new { productCategory.CategoryId, productCategory.ProductId });

        }
    }
}