namespace Restaurant.API.Models.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public int MenuItemId { get; set; }
        public MenuItemDTO MenuItem { get; set; }
        public int Quantity { get; set; }
    }
}
