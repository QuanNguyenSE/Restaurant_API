using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int MenuItemId { get; set; }
        public MenuItemDTO MenuItem { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
