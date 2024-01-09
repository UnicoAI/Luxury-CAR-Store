using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Utilities;
using static Store.Data.ApplicationDbContext;

namespace Store.Areas.Identity.Pages.Account.Manage.Orders
{
    public class IndexModel : PaginationModel<IndexModel.User>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context; // Assuming you have a DbContext

        public IndexModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class User
        {
            public string Id { get; set; } = default!;

            [Display(Name = "User ID")]
            public string DisplayId { get; set; } = default!;

            [Display(Name = "Username")]
            public string Username { get; set; } = default!;

            [Display(Name = "Roles")]
            public string Roles { get; set; } = default!;

            public bool CanDelete { get; set; }

            public List<Order> Orders { get; set; } = new List<Order>(); // Add this property
        }

        public async Task OnGetAsync(int? p, int? limit)
        {
            var query = _userManager.Users.AsNoTracking()
                .Select(user => new User
                {
                    Id = user.Id,
                    DisplayId = user.Id,
                    Username = user.UserName!,
                    Roles = "", // Initialize to empty, will be filled later
                    CanDelete = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != null && user.Id != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                });

            // Load roles separately to avoid conflicts with UserManager's DbContext
            foreach (var user in query)
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
                user.Roles = string.Join(", ", roles);
            }

            await LoadItemsAsync(query, p, limit ?? 5);

            foreach (var item in Items)
            {
                item.DisplayId = Utility.ConvertGuidToViewModel(item.DisplayId) ?? "Invalid-Id";

                // Retrieve user orders
                item.Orders = await _context.Orders
                    .Where(order => order.UserId == item.Id)
                    .ToListAsync();
            }
        }
    }
}
