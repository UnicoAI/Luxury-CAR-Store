using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = default!;

        public Order ProductId { get; set; }
      
        public Product Product { get; set; } = default!;

        public Cart Quantity { get; set; }

        // Add more properties as needed, such as price per item, subtotal, etc.
     
    }
}
