using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Orders
{
    public class DeleteModel : PageModel
    {
        private readonly Store.Data.ApplicationDbContext _context;

        public DeleteModel(Store.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Order_1 == null)
            {
                return NotFound();
            }

            var order = await _context.Order_1.FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            else 
            {
                Order = order;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Order_1 == null)
            {
                return NotFound();
            }
            var order = await _context.Order_1.FindAsync(id);

            if (order != null)
            {
                Order = order;
                _context.Order_1.Remove(Order);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
