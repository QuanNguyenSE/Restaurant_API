using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Restaurant.API.Utility;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        [Required]
        public int NumberOfGuests { get; set; }
        public string? SpecialRequest { get; set; } 
        public DateTime DateCreated { get; set; } = DateTime.UtcNow; 
        public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
