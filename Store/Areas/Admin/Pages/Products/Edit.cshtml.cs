using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EditModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty] public ProductModel Product { get; set; } = default!;
        public List<CategoryModel> Categories { get; set; }

        public class CategoryModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = default!;
        }

        public class ProductModel
        {
            public int Id { get; set; }

            [Display(Name = "Name")]
            [Required(ErrorMessage = "{0} is required.")]
            [MinLength(3, ErrorMessage = "Enter at least {1} characters.")]
            public string Name { get; set; } = default!;

            [Display(Name = "Categories")] public List<int> Categories { get; set; } = new();

            [Display(Name = "Price (£)")]
            [Range(0, double.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public decimal? Price { get; set; }

            [Display(Name = "Quantity")]
            [Range(0, int.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public int? StockQuantity { get; set; }

            [Display(Name = "Product Weight (kg)")]
            [Range(0, double.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public double? ProductWeight { get; set; }

            [Display(Name = "Image")] public IFormFile? Image { get; set; }
            public string ImageUrl { get; set; }

            [Display(Name = "Description")]
            [MaxLength(80, ErrorMessage = "{0} must be a maximum of {1} characters.")]
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is null)
                return NotFound();

            var product = await _context.Products.AsNoTracking()
                .Include(product => product.ProductCategories)
                .FirstOrDefaultAsync(product => product.Id == id);

            if (product is null)
                return NotFound();

            Product = new ProductModel
            {
                Id = product.Id,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ProductWeight = product.ProductWeight,
                Categories = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
                ImageUrl = product.Image
            };
            await LoadCategoriesAsync();

            return Page();
        }

        private async Task LoadCategoriesAsync()
        {
            Categories = await _context.Categories.AsNoTracking()
                .Select(item => new CategoryModel
                {
                    Id = item.Id,
                    Name = item.Name
                }).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var dbProduct = await _context.Products
                .Include(product => product.ProductCategories)
                .FirstOrDefaultAsync(product => product.Id == Product.Id);
            if (dbProduct is null)
            {
                await LoadCategoriesAsync();
                ModelState.AddModelError(string.Empty, "This item has been deleted!");
                return Page();
            }

            var isNewImageUploaded = Product.Image is not null;
            if (isNewImageUploaded && Product.Image!.Length < 100)
            {
                ModelState.AddModelError("Product.Image", "The image size is too small!");
                await LoadCategoriesAsync();
                return Page();
            }

            if (!Product.Categories.Any())
            {
                ModelState.AddModelError("Product.Categories", "Select at least one category.");
                await LoadCategoriesAsync();
                return Page();
            }

            if (!await _context.AllEntitiesExistAsync(Product.Categories))
            {
                ModelState.AddModelError(string.Empty,
                    "Categories are not valid. Some categories may have been deleted. Refresh the page and try again.");
                await LoadCategoriesAsync();
                return Page();
            }

            if (isNewImageUploaded)
            {
                // Generate a unique file name for the image
                var uniqueFileName = $"{Guid.NewGuid():N}_{Product.Image!.FileName}";

                var imageUrl = $"img/products/{uniqueFileName}";
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);

                // Save the image to the server
                await using var stream = new FileStream(imagePath, FileMode.Create);
                await Product.Image.CopyToAsync(stream);

                // Remove old image
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, dbProduct.Image[1..]);
                System.IO.File.Delete(oldImagePath);

                // Set new image path
                dbProduct.Image = $"/{imageUrl}";
            }

            dbProduct.Name = Product.Name;
            dbProduct.Description = Product.Description;
            dbProduct.Price = (decimal)Product.Price!;
            dbProduct.StockQuantity = (int)Product.StockQuantity!;
            dbProduct.ProductWeight = (double)Product.ProductWeight!;

            var productCategories = Product.Categories
                .Select(categoryId => new ProductCategory { CategoryId = categoryId, ProductId = dbProduct.Id })
                .ToList();
            dbProduct.ProductCategories = productCategories;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
