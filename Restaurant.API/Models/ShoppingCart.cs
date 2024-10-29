using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [NotMapped]
        public string StripePaymentIntentId { get; set; }
        [NotMapped]
        public string ClientSecret { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
