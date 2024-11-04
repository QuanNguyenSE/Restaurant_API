using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class OrderHeaderCreateDTO
    {
        [Required]
        public string ApplicationUserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public double OrderTotal { get; set; }
        public int ItemsTotal { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public string? PaymentIntentId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }


        public IEnumerable<OrderDetailsCreateDTO> OrderDetail { get; set; }
    }
}
