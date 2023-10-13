using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Store.Areas.Admin.Pages.Members
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public User UserModel { get; set; }

        public class User
        {
            [Display(Name = "ID")]
            public string Id { get; set; } = default!;

            [Display(Name = "Username")]
            public string Username { get; set; } = default!;

            [Display(Name = "Roles")]
            public string Roles { get; set; } = default!;

            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Display(Name = "Locked?")]
            public bool IsLockout { get; set; }

            [Display(Name = "Phone")]
            public string? Phone { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id is null)
                return NotFound();

            var user = await _userManager.Users.AsNoTracking()
                            .FirstOrDefaultAsync(user => user.Id == id);
            if (user is null)
                return NotFound();

            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == user.Id)
                return Forbid(); // Users cannot remove themselves.

            var roles = await _userManager.GetRolesAsync(user);

            UserModel = new User
            {
                Id = user.Id,
                Username = user.UserName!,
                Roles = string.Join(", ", roles),
                Phone = user.PhoneNumber,
                Email = user.Email,
                IsLockout = await _userManager.IsLockedOutAsync(user)
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (id is null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            var currentUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == user.Id)
                return Forbid(); // Users cannot remove themselves.

            await _userManager.DeleteAsync(user);
            return RedirectToPage("./Index");
        }
    }
}
