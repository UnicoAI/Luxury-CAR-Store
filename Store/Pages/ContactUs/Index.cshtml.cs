using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Store.Data;
using Store.Models;

namespace Store.Pages.ContactUs;

public class Index : PageModel {
    private readonly ApplicationDbContext _context;

    public Index(ApplicationDbContext context) {
        _context = context;
    }

    [BindProperty] public InputModel Input { get; set; }


    public class InputModel {
        [Display(Name = "Name")] public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Title")] public string? Title { get; set; }

        [MinLength(10, ErrorMessage = "Message to {1} short")]
        [Display(Name = "Message")]
        [Required(ErrorMessage = "Message {0} invalid .")]
        public string Message { get; set; } = default!;
    }

    public void OnGet() {
    }

    public async Task<IActionResult> OnPostAsync() {
        if (!ModelState.IsValid) {
            return Page();
        }

        _context.Messages.Add(new ContactUsMessage {
            Name    = Input.Name,
            Email   = Input.Email,
            Title   = Input.Title,
            Message = Input.Message,
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Success");
    }
}