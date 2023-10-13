using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Store.Data;
using Store.Models;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Store.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CheckoutModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Checkout CheckoutData { get; set; }

        public void OnGet()
        {
            var checkoutDataJson = TempData["CheckoutData"] as string;
            if (!string.IsNullOrEmpty(checkoutDataJson))
            {
                CheckoutData = JsonConvert.DeserializeObject<Checkout>(checkoutDataJson);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var order = new Order
            {
                UserId = userId,
                ProductId = 1, // Set the product ID as needed
                Quantity = 1, // Set the quantity as needed
                OrderDate = DateTime.Now,
                Status = OrderStatus.InProgress
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
