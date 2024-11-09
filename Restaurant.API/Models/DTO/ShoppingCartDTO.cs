namespace Restaurant.API.Models.DTO
{
    public class ShoppingCartDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public ICollection<CartItemDTO> CartItems { get; set; }
        public double CartTotal { get; set; }
        public int ItemsTotal { get; set; }

    }
}
