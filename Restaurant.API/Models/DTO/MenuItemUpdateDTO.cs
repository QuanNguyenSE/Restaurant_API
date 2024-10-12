using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class MenuItemUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? SpecialTag { get; set; }
        public string? Category { get; set; }
        [Range(1, int.MaxValue)]
        [Required]
        public double Price { get; set; }
        [ValidateNever]
        public IFormFile Image { get; set; }
    }
}
