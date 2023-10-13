using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages.Products
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ProductModel Product { get; set; } = default!;

        public class ProductModel
        {
            public int Id { get; set; }

            [Display(Name = "Name")] public string Name { get; set; } = default!;

            [Display(Name = "Categories")] public string ProductCategories { get; set; } = default!;

            [Display(Name = "Price £")]
            [DataType(DataType.Currency)]
            [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
            public decimal Price { get; set; }

            [Display(Name = "Quantity")] public int StockQuantity { get; set; }

            [Display(Name = "Product Weight (kg)")]
            public double ProductWeight { get; set; }

            [Display(Name = "Image")] public string Image { get; set; }

            [Display(Name = "Description")] public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(product => product.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (product == null)
                return NotFound();

            Product = new ProductModel
            {
                Id = product.Id,
                ProductCategories = string.Join(", ", product.ProductCategories.Select(pc => pc.Category.Name)),
                Image = product.Image,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                ProductWeight = product.ProductWeight,
                StockQuantity = product.StockQuantity
            };

            return Page();
        }
    }
}
