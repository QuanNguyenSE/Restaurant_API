using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? SpecialTag { get; set; }
        [Range(1, int.MaxValue)]
        [Required]
        public double Price { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        // Foreign key to Category
        [Required]
        public int CategoryId { get; set; }

        // Navigation property to Category
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
    }
}
