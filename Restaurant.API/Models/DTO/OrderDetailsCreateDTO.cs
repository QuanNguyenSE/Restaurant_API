using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class OrderDetailsCreateDTO
    {
        [Required]
        public int OrderHeaderId { get; set; }
        [Required]
        public int MenuItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
