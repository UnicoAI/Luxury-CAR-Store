using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Store.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        // Corrected: Renamed ProductId to refer to the Product entity
        public int ProductId { get; set; }

        // Corrected: Changed the name to reflect that it's a navigation property
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
