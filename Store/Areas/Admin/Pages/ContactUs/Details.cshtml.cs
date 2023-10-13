using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages.ContactUs
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public MessageModel Message { get; set; } = default!;

        public class MessageModel
        {
            [Display(Name = "ID")] public int Id { get; set; }
            [Display(Name = "Name")] public string? Name { get; set; }

            [Display(Name = "Email")] public string? Email { get; set; }

            [Display(Name = "Title")] public string? Title { get; set; }

            [Display(Name = "Message")] public string Message { get; set; } = default!;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
                return NotFound();

            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
            if (message is null)
                return NotFound();

            Message = new MessageModel
            {
                Id = message.Id,
                Message = message.Message,
                Email = message.Email,
                Name = message.Name,
                Title = message.Title
            };

            return Page();
        }
    }
}
