using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [NotMapped]
        public string StripePaymentIntentId { get; set; }
        [NotMapped]
        public string ClientSecret { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
        [NotMapped]
        public int ItemsTotal { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
