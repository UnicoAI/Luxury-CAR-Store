using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Store.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; } = default!;

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Cart Product { get; set; } = default!;

        public Cart Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<Order> Orders { get; set; } // Navigation property


        // Add a navigation property for OrderDetails

    }

    public enum OrderStatus
    {
        InProgress,
        Completed,
        Canceled
    }
}

