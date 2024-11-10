namespace Restaurant.API.Models.DTO
{
    public class OrderHeaderDTO
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderDetailDTO> OrderDetail { get; set; }
        public int ItemsTotal { get; set; }
        public double OrderTotal { get; set; }
        public double DeliveryFee { get; set; }
        public string OrderStatus { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }
    }
}
