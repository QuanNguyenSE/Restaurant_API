using Restaurant.API.Utility;

namespace Restaurant.API.Models.DTO
{
    public class OrderHeaderUpdateDTO
    {
        public int Id { get; set; }
        public string? OrderStatus { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }
    }
}
