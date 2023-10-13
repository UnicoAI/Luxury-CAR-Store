using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Store.Models;

public class Cart {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string UserId { get; set; } = default!;
    public IdentityUser User { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }
    public List<Order> Orders { get; set; } // Navigation property

    public bool IsArchived { get; set; }

    public static implicit operator Cart(int v)
    {
        throw new NotImplementedException();
    }
}