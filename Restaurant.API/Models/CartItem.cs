using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        [ValidateNever]
        public MenuItem MenuItem { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Price { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
