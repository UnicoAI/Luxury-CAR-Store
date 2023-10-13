using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Utilities;

namespace Store.Areas.Admin.Pages.Categories;

public class IndexModel : PaginationModel<IndexModel.CategoryModel> {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public class CategoryModel {
        [Display(Name = "Id")] public int Id { get; set; }

        [Display(Name = "Name")] public string Name { get; set; }
        [Display(Name = "Product Number")] public int NumOfProducts { get; set; }
    }

    public async Task OnGetAsync(int? p, int? limit) {
        var query = _context.Categories.AsNoTracking()
            .Include(category => category.ProductCategories)
            .OrderByDescending(category => category.Id)
            .Select(item => new CategoryModel {
                Id            = item.Id,
                Name          = item.Name,
                NumOfProducts = item.ProductCategories.Count
            });
        await LoadItemsAsync(query, p, limit ?? 10);
    }
}