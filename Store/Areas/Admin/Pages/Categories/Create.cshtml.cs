using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Categories;

public class CreateModel : PageModel {
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context) {
        _context = context;
    }

    public IActionResult OnGet() {
        return Page();
    }

    [BindProperty] public CategoryModel Category { get; set; } = default!;

    public class CategoryModel {
        [Display(Name = "Category Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [MinLength(3, ErrorMessage = "{0} must be at least {1} characters.")]

        public string Name { get; set; }
    }


    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid)
            return Page();

        _context.Categories.Add(new Category {
            Name = Category.Name
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}