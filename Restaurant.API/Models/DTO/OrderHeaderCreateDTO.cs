using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class OrderHeaderCreateDTO
    {
        public string ApplicationUserId { get; set; }
        [Required]
        public DeliveryInfo DeliveryInfo { get; set; }

    }
}
