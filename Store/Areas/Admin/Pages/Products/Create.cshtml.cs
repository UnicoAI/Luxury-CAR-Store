using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;
using Microsoft.AspNetCore.Hosting;

namespace Store.Areas.Admin.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public ProductModel Product { get; set; } = new();
        public List<CategoryModel> Categories { get; set; }

        public class CategoryModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = default!;
        }

        public class ProductModel
        {
            [Display(Name = "Name")]
            [Required(ErrorMessage = "{0} is required.")]
            [MinLength(3, ErrorMessage = "Enter at least {1} characters.")]
            public string Name { get; set; } = default!;

            [Display(Name = "Categories")]
            public List<int> Categories { get; set; } = new();

            [Display(Name = "Price £")]
            [Range(0, double.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public decimal? Price { get; set; }

            [Display(Name = "Quantity")]
            [Range(0, int.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public int? StockQuantity { get; set; }

            [Display(Name = "Product Weight (Kg)")]
            [Range(0, double.MaxValue, ErrorMessage = "{0} must be between {1} and {2}.")]
            [Required(ErrorMessage = "{0} is required.")]
            public double? ProductWeight { get; set; }

            [Required(ErrorMessage = "{0} is required.")]
            [Display(Name = "Image")]
            public IFormFile Image { get; set; }

            [Display(Name = "Description")]
            [MaxLength(80, ErrorMessage = "{0} must be a maximum of {1} characters.")]
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
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

            if (Product.Image.Length < 100)
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
                    "Categories are not valid. It's possible that a category has been deleted. Refresh the page and try again.");
                await LoadCategoriesAsync();
                return Page();
            }

            // Generate a unique file name for the image
            var uniqueFileName = $"{Guid.NewGuid():N}_{Product.Image.FileName}";

            var imageUrl = $"img/products/{uniqueFileName}";
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl);

            // Save the image to the server
            await using (var stream = new FileStream(imagePath, FileMode.Create))
                await Product.Image.CopyToAsync(stream);

            var product = new Product
            {
                Name = Product.Name,
                Description = Product.Description,
                Price = (decimal)Product.Price!,
                StockQuantity = (int)Product.StockQuantity!,
                Image = $"/{imageUrl}",
                ProductWeight = (double)Product.ProductWeight!,
            };

            var productCategories = Product.Categories
                .Select(categoryId => new ProductCategory { CategoryId = categoryId })
                .ToList();
            product.ProductCategories.AddRange(productCategories);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var productCategory in productCategories)
                productCategory.ProductId = product.Id;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
