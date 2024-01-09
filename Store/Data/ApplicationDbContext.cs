using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Order> Orders { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ContactUsMessage> Messages { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public Product Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductCategory>()
            .HasKey(productCategory => new { productCategory.CategoryId, productCategory.ProductId });
    }

    public DbSet<Store.Models.Order> Order_1 { get; set; } = default!;

  
}