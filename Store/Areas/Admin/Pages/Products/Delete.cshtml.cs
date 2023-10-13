using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Store.Areas.Admin.Pages.Products
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public ProductModel Product { get; set; } = default!;

        public class ProductModel
        {
            public int Id { get; set; }

            [Display(Name = "Name")]
            public string Name { get; set; } = default!;

            [Display(Name = "Categories")]
            public string ProductCategories { get; set; } = default!;

            [Display(Name = "Price £")]
            [DataType(DataType.Currency)]
            [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
            public decimal Price { get; set; }

            [Display(Name = "Quantity")]
            public int StockQuantity { get; set; }


            [Display(Name = "Product Weight (Kg)")]
            public double ProductWeight { get; set; }

            [Display(Name = "Image")]
            public string Image { get; set; }
            [Display(Name = "Description")]
            public string? Description { get; set; }
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.Products.FindAsync(id);
            if (item is null)
                return RedirectToPage("./Index");

            _context.Products.Remove(item);
            await _context.SaveChangesAsync();

            // Remove old image
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, item.Image[1..]);
            System.IO.File.Delete(oldImagePath);

            return RedirectToPage("./Index");
        }
    }
}
