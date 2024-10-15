using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
