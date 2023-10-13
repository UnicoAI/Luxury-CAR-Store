namespace Store.Models
{
    public class Checkout
    {
        public List<Cart> Carts { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
