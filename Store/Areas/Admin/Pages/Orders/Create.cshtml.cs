using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly Store.Data.ApplicationDbContext _context;

        public CreateModel(Store.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Order_1 == null || Order == null)
            {
                return Page();
            }

            _context.Order_1.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
