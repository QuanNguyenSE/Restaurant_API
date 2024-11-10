using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Restaurant.API.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public int ItemsTotal { get; set; }
        public double OrderTotal { get; set; }
        public double DeliveryFee { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DeliveryInfo? DeliveryInfo { get; set; }

        //public DateTime PaymentDate { get; set; }
        //public DateTime? ShippingDate { get; set; }
        //public string? PaymentStatus { get; set; }
        //public string? TrackingNumber { get; set; }
        //public string? Carrier { get; set; }
        //public string? PaymentIntentId { get; set; }

    }
}
